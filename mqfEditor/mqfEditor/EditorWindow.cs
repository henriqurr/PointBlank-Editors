using mqfDecryptor.Enums.Items;
using mqfDecryptor.Enums.Mission;
using mqfDecryptor.Enums.Weapon;
using mqfDecryptor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.ListViewItem;

namespace mqfDecryptor
{
    public partial class EditorWindow : Form
    {
        private static List<MissionItemAward> _items = new List<MissionItemAward>();
        private static List<Card> list = new List<Card>();
        private static List<CardAwards> awards = new List<CardAwards>();

        private static MqfFile mqfFile = new MqfFile();
        public EditorWindow()
        {
            InitializeComponent();
        }
        private void EditorWindow_Load(object sender, EventArgs e)
        {
            fdOpenFile.Filter = "mqf Files|*.mqf";
            fdOpenFile.FileName = "";
        }

        private void LoadMqf(string path, int typeLoad)
        {
            byte[] buffer = File.ReadAllBytes(path);
            int missionId = 13;
            try
            {
                ReceiveGPacket r = new ReceiveGPacket(buffer);
                mqfFile.mqfFileObjects.fileFormat = r.ReadBytes(4);
                mqfFile.mqfFileObjects.questType = r.ReadInt();
                mqfFile.mqfFileObjects.unkBytes = r.ReadBytes(16);
                mqfFile.mqfFileObjects.cardObj = new List<CardObjects>();
                mqfFile.mqfFileObjects.cardRewards2 = new CardRewards2();
                int valor1 = 0, valor2 = 0;
                for (int i = 0; i < 40; i++)
                {
                    int missionBId = valor2++,
                        cardBId = valor1;
                    if (valor2 == 4)
                    {
                        valor2 = 0;
                        valor1++;
                    }
                    CardObjects card = new CardObjects
                    {
                        reqType = r.ReadUshort(),
                        type = r.ReadByte(),
                        mapId = r.ReadByte(),
                        limitCount = r.ReadByte(),
                        weaponClass = r.ReadByte(),
                        weaponId = r.ReadUshort()
                    };
                    Card nc = new Card(cardBId, missionBId)
                    {
                        _mapId = card.mapId,
                        _weaponReq = (ClassType)card.weaponClass,
                        _weaponReqId = card.weaponId,
                        _missionType = (MISSION_TYPE)card.type,
                        _missionLimit = card.limitCount,
                        _missionId = missionId
                    };
                    Console.WriteLine(string.Format("mapId:{0} weaponReq:{1} weaponReqId:{2} missionType:{3} missionLimit:{4} missionId:{5}", nc._mapId, nc._weaponReq, nc._weaponReqId, nc._missionType, nc._missionLimit, nc._missionId));
                    list.Add(nc);
                    if (mqfFile.mqfFileObjects.questType == 1)
                    {
                        card.unkBytes1 = r.ReadBytes(24);
                    }
                    mqfFile.mqfFileObjects.cardObj.Add(card);
                }
                int vai = (mqfFile.mqfFileObjects.questType == 2 ? 5 : 1); // se questType for igual a 2 repete o for 5 vezes senão repete 1
                mqfFile.mqfFileObjects.cardRewards = new List<CardRewards>();
                for (int i = 0; i < 10; i++)
                {
                    CardRewards cardRewards = new CardRewards
                    {
                        gp = r.ReadInt(),
                        xp = r.ReadInt(),
                        medals = r.ReadInt()
                    };
                    Console.WriteLine(string.Format("gp:{0} exp:{1} medals:{2}", cardRewards.gp, cardRewards.xp, cardRewards.medals));
                    cardRewards.cardRewardsObjects = new List<CardRewardsObjects>();
                    for (int i2 = 0; i2 < vai; i2++)
                    {
                        CardRewardsObjects cardRewardsObjects = new CardRewardsObjects
                        {
                            unk = r.ReadInt(),
                            type = r.ReadInt(),
                            itemId = r.ReadInt(),
                            itemCount = r.ReadInt()
                        };
                        Console.WriteLine(string.Format("unk:{0} type:{1} itemId:{2} itemCount:{3}", cardRewardsObjects.unk, cardRewardsObjects.type, cardRewardsObjects.itemId, cardRewardsObjects.itemCount));
                        cardRewards.cardRewardsObjects.Add(cardRewardsObjects);
                    }
                    if (typeLoad == 1)
                    {
                        CardAwards card = new CardAwards { _id = missionId, _card = i, _exp = (mqfFile.mqfFileObjects.questType == 1 ? (cardRewards.xp * 10) : cardRewards.xp), _gp = cardRewards.gp };
                        GetCardMedalInfo(card, cardRewards.medals);
                        if (!card.Unusable())
                            awards.Add(card);
                    }
                    mqfFile.mqfFileObjects.cardRewards.Add(cardRewards);
                }
                if (mqfFile.mqfFileObjects.questType == 2)
                {
                    mqfFile.mqfFileObjects.cardRewards2.goldResult = r.ReadInt();
                    mqfFile.mqfFileObjects.cardRewards2.unkBytes = r.ReadBytes(8);
                    mqfFile.mqfFileObjects.cardRewards2.cardRewardsObjects = new List<CardRewards2Objects>();
                    for (int i = 0; i < 5; i++)
                    {
                        CardRewards2Objects cardRewardsObjects = new CardRewards2Objects
                        {
                            unkI = r.ReadInt(),
                            itemType = r.ReadInt(), //1 - unidade | 2 - dias
                            itemId = r.ReadInt(),
                            itemCount = r.ReadInt()
                        };
                        Console.WriteLine("Unk:{0} ItemType:{1} ItemId:{2} ItemCount:{3}", cardRewardsObjects.unkI, cardRewardsObjects.itemType, cardRewardsObjects.itemId, cardRewardsObjects.itemCount);
                        if (cardRewardsObjects.unkI > 0)
                        {
                            MissionItemAward mission = new MissionItemAward
                            {
                                _missionId = missionId,
                                item = new ItemsModel(cardRewardsObjects.itemId)
                                {
                                    _equip = 1,
                                    _count = (uint)cardRewardsObjects.itemCount,
                                    _name = "Mission item"
                                }
                            };
                            _items.Add(mission);
                            ListViewItem item = new ListViewItem(); // Unk
                            ListViewSubItem listViewSubItem1 = new ListViewSubItem(); //itemType
                            ListViewSubItem listViewSubItem2 = new ListViewSubItem(); //itemId
                            ListViewSubItem listViewSubItem3 = new ListViewSubItem(); //ItemCount
                            item.Text = cardRewardsObjects.unkI.ToString();
                            listViewSubItem1.Text = cardRewardsObjects.itemType.ToString();
                            listViewSubItem2.Text = cardRewardsObjects.itemId.ToString();
                            listViewSubItem3.Text = cardRewardsObjects.itemCount.ToString();
                            item.SubItems.Add(listViewSubItem1);
                            item.SubItems.Add(listViewSubItem2);
                            item.SubItems.Add(listViewSubItem3);
                            rewardsView.Items.Add(item);
                        }
                        mqfFile.mqfFileObjects.cardRewards2.cardRewardsObjects.Add(cardRewardsObjects);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("[MissionCardXML] Erro no arquivo: " + "\r\n" + ex.ToString());
            }
        }

        private void WriteMqfFile(string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(fileStream))
                {
                    writer.Write(mqfFile.mqfFileObjects.fileFormat);
                    writer.Write(mqfFile.mqfFileObjects.questType);
                    writer.Write(mqfFile.mqfFileObjects.unkBytes);
                    int valor1 = 0, valor2 = 0;
                    for (int i = 0; i < 40; i++)
                    {
                        CardObjects obj = mqfFile.mqfFileObjects.cardObj[i];
                        if (valor2 == 4)
                        {
                            valor2 = 0;
                            valor1++;
                        }
                        writer.Write(obj.reqType);
                        writer.Write(obj.type);
                        writer.Write(obj.mapId);
                        writer.Write(obj.limitCount);
                        writer.Write(obj.weaponClass);
                        writer.Write(obj.weaponId);
                        if (mqfFile.mqfFileObjects.questType == 1)
                        {
                            writer.Write(obj.unkBytes1);
                        }
                    }
                    int vai = (mqfFile.mqfFileObjects.questType == 2 ? 5 : 1); // se questType for igual a 2 repete o for 5 vezes senão repete 1
                    for (int i = 0; i < 10; i++)
                    {
                        CardRewards cardRewards = mqfFile.mqfFileObjects.cardRewards[i];
                        writer.Write(cardRewards.gp);
                        writer.Write(cardRewards.xp);
                        writer.Write(cardRewards.medals);

                        for (int i2 = 0; i2 < vai; i2++)
                        {
                            CardRewardsObjects cardRewardsObjects = cardRewards.cardRewardsObjects[i2];
                            writer.Write(cardRewardsObjects.unk);
                            writer.Write(cardRewardsObjects.type);
                            writer.Write(cardRewardsObjects.itemId);
                            writer.Write(cardRewardsObjects.itemCount);
                        }
                    }
                    if (mqfFile.mqfFileObjects.questType == 2)
                    {
                        writer.Write(mqfFile.mqfFileObjects.cardRewards2.goldResult);
                        writer.Write(mqfFile.mqfFileObjects.cardRewards2.unkBytes);
                        for (int i = 0; i < 5; i++)
                        {
                            CardRewards2Objects cardRewards2Objects = mqfFile.mqfFileObjects.cardRewards2.cardRewardsObjects[i];
                            writer.Write(cardRewards2Objects.unkI);
                            writer.Write(cardRewards2Objects.itemType);
                            writer.Write(cardRewards2Objects.itemId);
                            writer.Write(cardRewards2Objects.itemCount);
                        }
                    }
                    fileStream.Close();
                    writer.Close();
                }
            }
        }

        private static void GetCardMedalInfo(CardAwards card, int medalId)
        {
            if (medalId == 0)
                return;
            if (medalId >= 1 && medalId <= 50) //v >= 1 && v <= 50
                card._brooch++;
            else if (medalId >= 51 && medalId <= 100) //v >= 51 && v <= 100
                card._insignia++;
            else if (medalId >= 101 && medalId <= 116) //v >= 101 && v <= 116
                card._medal++;
            //v >= 117 && v <= 239
        }

        public static CardAwards getAward(int mission, int cartao)
        {
            for (int i = 0; i < awards.Count; i++)
            {
                CardAwards card = awards[i];
                if (card._id == mission && card._card == cartao)
                    return card;
            }
            return null;
        }

        public static string FromHexString(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return Encoding.Unicode.GetString(bytes); // returns: "Hello world" for "48656C6C6F20776F726C64"
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WriteMqfFile(fdOpenFile.FileName);
            MessageBox.Show(string.Format("Salvo com sucesso. Path:{0}", fdOpenFile.FileName));
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("All credits to Exploit Network\nCreated by Darkness", "Credits");
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rewardsView.FocusedItem == null)
            {
                return;
            }
            ListViewItem item = rewardsView.FocusedItem;
            txtUnk.Text = item.SubItems[0].Text;
            txtItemType.Text = item.SubItems[1].Text;
            txtItemId.Text = item.SubItems[2].Text;
            txtItemCount.Text = item.SubItems[3].Text;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = fdOpenFile.FileName.Substring(fdOpenFile.FileName.LastIndexOf("\\") + 1);
            saveFileDialog.Filter = string.Format("{0}|{0}", fdOpenFile.FileName.Substring(fdOpenFile.FileName.LastIndexOf("\\") + 1));
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                WriteMqfFile(saveFileDialog.FileName);
                MessageBox.Show(string.Format("Salvo com sucesso. Path:{0}", fdOpenFile.FileName));
            }
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            btnOpenFile_Click(sender, e);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (rewardsView.FocusedItem == null)
            {
                return;
            }
            ListViewItem item = rewardsView.FocusedItem;
            item.SubItems[0].Text = txtUnk.Text;
            item.SubItems[1].Text = txtItemType.Text;
            item.SubItems[2].Text = txtItemId.Text;
            item.SubItems[3].Text = txtItemCount.Text;
            CardRewards2Objects card = new CardRewards2Objects
            {
                unkI = int.Parse(txtUnk.Text),
                itemType = int.Parse(txtItemType.Text),
                itemId = int.Parse(txtItemId.Text),
                itemCount = int.Parse(txtItemCount.Text)
            };
            mqfFile.mqfFileObjects.cardRewards2.cardRewardsObjects[rewardsView.FocusedItem.Index] = card;
            txtUnk.Clear();
            txtItemType.Clear();
            txtItemId.Clear();
            txtItemCount.Clear();
            MessageBox.Show("Item editado com sucesso!");
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            if (fdOpenFile.ShowDialog() == DialogResult.OK)
            {
                txtFile.Text = fdOpenFile.FileName;
                mqfFile.mqfFileObjects = new MqfFileObjects();
                mqfFile.mqfFileObjects.cardObj = new List<CardObjects>();
                mqfFile.mqfFileObjects.cardRewards = new List<CardRewards>();
                mqfFile.mqfFileObjects.cardRewards2 = new CardRewards2();
                rewardsView.Items.Clear();
                txtUnk.Clear();
                txtItemType.Clear();
                txtItemId.Clear();
                txtItemCount.Clear();
                LoadMqf(fdOpenFile.FileName, 1);
            }
        }
    }
}
