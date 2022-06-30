using PropertyEditor.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PropertyEditor.Managers
{
    public class ObjectsManager
    {
        public static List<Objects> _objects = new List<Objects>();
        public static Dictionary<ulong, ulong> _changeOffsets;
        public static Dictionary<ulong, Objects> _editSaved = new Dictionary<ulong, Objects>();
        public static Dictionary<ulong, Objects> _loadSaved = new Dictionary<ulong, Objects>();

        public static void GetObjects(BinaryReader reader, Header header)
        {
            Console.WriteLine("Loading {0} objects...", header.ObjectInfoCount);
            reader.BaseStream.Position = (long)header.ObjectInfoOffset;
            for (int i = 0; i < header.ObjectInfoCount; i++)
            {
                Objects obj = new Objects
                {
                    Type = reader.ReadInt32(),
                    Id = reader.ReadUInt64(),
                    Offset = reader.ReadUInt64(),
                    Size = reader.ReadUInt64()
                };
                _objects.Add(obj);
                Program._propertyEditor.SetProgressBarValue(i + 1, header.ObjectInfoCount);
            }
        }

        public static Objects GetObjectById(ulong id)
        {
            return _objects.Where(x => x.Id == id).FirstOrDefault();
        }

        public static Objects GetRegistryRoot()
        {
            return _objects.Where(x => x.Keys.IsRegistryRoot).FirstOrDefault();
        }

        public static void GetObjectsValues(BinaryReader reader)
        {
            Console.WriteLine("Loading {0} object values...", _objects.Count);
            for (int i = 0; i < _objects.Count; i++)
            {
                Objects obj = _objects[i];
                reader.BaseStream.Position = (long)obj.Offset;
                obj.Keys = new ObjectsValues
                {
                    Folders = new List<ulong>(),
                    Items = new List<ulong>(),
                    Nations = new List<object>()
                };
                int lenghtName = reader.ReadByte();
                obj.Keys.Name = Encoding.GetEncoding(Settings.Encoding).GetString(reader.ReadBytes(lenghtName));
                if (!ObjectIsFolder(reader, obj))
                {
                    obj.Keys.IsFolder = false;
                    reader.BaseStream.Position -= 4; //se n for TRN3 volta 4 bytes pra pegar o item type
                    obj.Keys.Type = reader.ReadInt32();
                    obj.Keys.ValueType = obj.Keys.Type == 9 ? reader.ReadInt32() : obj.Keys.Type;
                    obj.Keys.NationsCount = obj.Keys.Type == 9 ? reader.ReadInt32() : 1;
                    GetValuesByNations(reader, obj); //puxa valores 
                }
                Program._propertyEditor.SetProgressBarValue(i + 1, _objects.Count);
            }
        }

        private static bool ObjectIsFolder(BinaryReader reader, Objects obj)
        {
            if (reader.ReadInt32() == 860770900) //TRN3 IS FOLDER
            {
                obj.Keys.IsFolder = true;
                reader.ReadInt16();
                obj.Keys.IsRegistryRoot = Convert.ToBoolean(reader.ReadInt32()); //isRegistryRoot
                reader.ReadUInt16();
                int foldersCount = reader.ReadInt32();
                reader.ReadBytes(60);
                for (int i = 0; i < foldersCount; i++)
                {
                    ulong id = (ulong)reader.ReadInt32(); //id of folders with items
                    obj.Keys.Folders.Add(id);
                }
                reader.ReadBytes(4); //rgk1
                int itemsCount = reader.ReadInt32();
                reader.ReadUInt64();
                for (int i = 0; i < itemsCount; i++)
                {
                    ulong id = reader.ReadUInt64(); //id of items in folder
                    obj.Keys.Items.Add(id);
                }
                return true;
            }
            return false;
        }

        private static void GetValuesByNations(BinaryReader reader, Objects obj)
        {
            string result = ((Models.Enums.ValueType)(uint)obj.Keys.ValueType).ToString();
            for (int i = 0; i < obj.Keys.NationsCount; i++)
            {
                switch (result)
                {
                    case "ROOT":
                        {
                            MessageBox.Show("Root Obj offset:" + obj.Offset);
                            int value = reader.ReadInt32();
                            obj.Keys.Nations.Add(value);
                            break;
                        }
                    case "INT32":
                        {
                            int value = reader.ReadInt32();
                            obj.Keys.Nations.Add(value);
                            break;
                        }
                    case "REAL32":
                        {
                            float value = reader.ReadSingle();
                            obj.Keys.Nations.Add(value);
                            break;
                        }
                    case "STRING":
                        {
                            reader.ReadBytes(4); //RGS3
                            uint lengthValue = reader.ReadUInt32();
                            string value = ReadUString(reader, (int)lengthValue * 2);
                            obj.Keys.Nations.Add(value);
                            break;
                        }
                    case "VEC2D":
                        {
                            float x = reader.ReadSingle();
                            float y = reader.ReadSingle();
                            var values = new
                            {
                                x,
                                y
                            };
                            obj.Keys.Nations.Add(values);
                            break;
                        }
                    case "VEC3D":
                        {
                            float x = reader.ReadSingle();
                            float y = reader.ReadSingle();
                            float z = reader.ReadSingle();
                            var values = new
                            {
                                x,
                                y,
                                z
                            };
                            obj.Keys.Nations.Add(values);
                            break;
                        }
                    case "VEC4D":
                        {
                            float x = reader.ReadSingle();
                            float y = reader.ReadSingle();
                            float z = reader.ReadSingle();
                            float w = reader.ReadSingle();
                            var values = new
                            {
                                x,
                                y,
                                z,
                                w
                            };
                            obj.Keys.Nations.Add(values);
                            break;
                        }
                    case "COLOR":
                        {
                            string value = BitConverter.ToString(reader.ReadBytes(4)).Replace("-", "");
                            obj.Keys.Nations.Add(value);
                            break;
                        }
                    case "MATRIX":
                        {
                            int value = reader.ReadInt32();
                            obj.Keys.Nations.Add(value);
                            break;
                        }
                    default:
                        {
                            int value = reader.ReadInt32();
                            obj.Keys.Nations.Add(value);
                            break;
                        }
                }
            }
        }

        public static void WriteObjects(BinaryWriter bw)
        {
            Console.WriteLine("Writting objects informations...");
            _changeOffsets = new Dictionary<ulong, ulong>();
            for (int i = 0; i < _objects.Count; i++)
            {
                Objects obj = _objects[i];
                bw.Write(obj.Type);
                bw.Write(obj.Id);
                _changeOffsets.Add(obj.Id, (ulong)bw.BaseStream.Position); //register offset to change again
                bw.Write(obj.Offset);
                bw.Write(obj.Size);
            }
        }

        public static void SetOffsets(BinaryWriter bw)
        {
            Console.WriteLine("Setting new offsets {0}/{1}...", _changeOffsets.Count, _objects.Count);
            int i = 0;
            foreach (KeyValuePair<ulong, ulong> item in _changeOffsets)
            {
                i++;
                Objects objModel = GetObjectById(item.Key);
                if (objModel != null)
                {
                    if (objModel.Offset == objModel.NewOffset)
                        continue;
                    //Console.WriteLine("Set new offset old:{0} new:{1}", objModel.Offset, objModel.NewOffset);
                    bw.BaseStream.Position = (long)item.Value;
                    byte[] newOffset = BitConverter.GetBytes(objModel.NewOffset);
                    bw.BaseStream.Write(newOffset, 0, 8);
                }
                if (Program._propertyEditor.pbTotal.ProgressBar.InvokeRequired)
                {
                    Program._propertyEditor.pbTotal.ProgressBar.Invoke(new Action(() =>
                    {
                        Program._propertyEditor.SetProgressBarValue(i + 1, _changeOffsets.Count);
                    }));
                }
                else
                {
                    Program._propertyEditor.SetProgressBarValue(i + 1, _changeOffsets.Count);
                }
                Application.DoEvents();
            }
            _changeOffsets.Clear(); //clear new list offsets
        }
		
        public static void WriteObjectsKeys(BinaryWriter bw)
        {
            Console.WriteLine("Writting objects keys {0}...", _objects.Count);
            for (int i = 0; i < _objects.Count; i++)
            {
                Objects obj = _objects[i];
                obj.NewOffset = (ulong)bw.BaseStream.Position; //change new offset to objects
                if(obj.Offset == obj.NewOffset)
                    _changeOffsets.Remove(obj.Id);
                bw.Write((byte)obj.Keys.Name.Length);
                bw.Write(Encoding.GetEncoding(Settings.Encoding).GetBytes(obj.Keys.Name));
                if (obj.Keys.IsFolder)
                {
                    WriteFolder(bw, obj);
                }
                else
                {
                    bw.Write(obj.Keys.Type);
                    if(obj.Keys.Type == 9)
                    {
                        bw.Write(obj.Keys.ValueType);
                        bw.Write(obj.Keys.NationsCount);
                    }
                    WriteValuesByNations(bw, obj); //escreve valores 
                }
                if (Program._propertyEditor.pbTotal.ProgressBar.InvokeRequired)
                {
                    Program._propertyEditor.pbTotal.ProgressBar.Invoke(new Action(() =>
                    {
                        Program._propertyEditor.SetProgressBarValue(i + 1, _objects.Count);
                    }));
                }
                else
                {
                    Program._propertyEditor.SetProgressBarValue(i + 1, _objects.Count);
                }
                Application.DoEvents();
            }
        }

        private static void WriteFolder(BinaryWriter bw, Objects obj)
        {
            bw.Write(Encoding.GetEncoding(Settings.Encoding).GetBytes("TRN3"));
            bw.Write((ushort)0);
            bw.Write(obj.Keys.IsRegistryRoot ? 1 : 0);
            bw.Write((ushort)0);
            bw.Write(obj.Keys.Folders.Count);
            bw.Write(new byte[60]);
            for (int i = 0; i < obj.Keys.Folders.Count; i++)
            {
                ulong id = obj.Keys.Folders[i]; // id de pastas com itens??
                bw.Write((int)id);
            }
            bw.Write(Encoding.GetEncoding(Settings.Encoding).GetBytes("RGK1"));
            bw.Write((int)obj.Keys.Items.Count);
            bw.Write((ulong)0);
            for (int i = 0; i < obj.Keys.Items.Count; i++)
            {
                ulong _id = obj.Keys.Items[i]; // id dos itens q ficam dentro da pasta ??
                bw.Write(_id);
            }
        }

        private static void WriteValuesByNations(BinaryWriter bw, Objects model)
        {
            string result = ((Models.Enums.ValueType)model.Keys.ValueType).ToString();
            for (int i = 0; i < model.Keys.Nations.Count; i++)
            {
                dynamic obj = model.Keys.Nations[i]; // dynamic
                switch (result)
                {
                    case "ROOT":
                        {
                            bw.Write(Convert.ToInt32(obj));
                            break;
                        }
                    case "INT32":
                        {
                            bw.Write(Convert.ToInt32(obj));
                            break;
                        }
                    case "REAL32":
                        {
                            bw.Write(float.Parse(obj.ToString()));
                            break;
                        }
                    case "STRING":
                        {
                            bw.Write(Encoding.GetEncoding(Settings.Encoding).GetBytes("RGS3"));
                            bw.Write((int)obj.ToString().Length);
                            bw.Write(Encoding.Unicode.GetBytes(obj.ToString()));
                            break;
                        }
                    case "VEC2D":
                        {

                            float x = obj.x;
                            float y = obj.y;
                            bw.Write((float)x);
                            bw.Write((float)y);
                            break;
                        }
                    case "VEC3D":
                        {
                            float x = obj.x;
                            float y = obj.y;
                            float z = obj.z;
                            bw.Write((float)x);
                            bw.Write((float)y);
                            bw.Write((float)z);
                            break;
                        }
                    case "VEC4D":
                        {
                            float x = obj.x;
                            float y = obj.y;
                            float z = obj.z;
                            float w = obj.w;
                            bw.Write((float)x);
                            bw.Write((float)y);
                            bw.Write((float)z);
                            bw.Write((float)w);
                            break;
                        }
                    case "COLOR":
                        {
                            for (int j = 0; j < 8; j += 2)
                                bw.Write(byte.Parse(obj.ToString().Substring(j, 2), NumberStyles.HexNumber));
                            break;
                        }
                    case "MATRIX":
                        {
                            bw.Write(Convert.ToInt32(obj));
                            break;
                        }
                    default:
                        {
                            bw.Write(Convert.ToInt32(obj));
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// EDITED PEFS
        /// </summary>
        /// <param name="bw">Buffer</param>
        /// <returns></returns>
        /// 
        public static void SaveEdited(BinaryWriter bw)
        {
            if (_editSaved.Count == 0)
                return;
            bw.Write(_editSaved.Count); //int
            bw.Write((int)Settings.Nation);
            foreach (Objects obj in _editSaved.Values)
            {
                bw.Write(obj.Type);
                bw.Write(obj.Id);
                bw.Write(obj.Offset);
                bw.Write(obj.Size);
                bw.Write((byte)obj.Keys.Name.Length);
                bw.Write(Encoding.GetEncoding(Settings.Encoding).GetBytes(obj.Keys.Name));
                if (!obj.Keys.IsFolder)
                {
                    bw.Write(obj.Keys.Type);
                    if (obj.Keys.Type == 9)
                    {
                        bw.Write(obj.Keys.ValueType);
                        bw.Write(obj.Keys.NationsCount);
                    }
                    WriteValuesByNations(bw, obj);
                }
            }
        }

        public static void LoadEdited(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            reader.ReadInt32(); //nation
            for (int i = 0; i < count; i++)
            {
                Objects obj = new Objects
                {
                    Type = reader.ReadInt32(),
                    Id = reader.ReadUInt64(),
                    Offset = reader.ReadUInt64(),
                    Size = reader.ReadUInt64()
                };
                obj.Keys = new ObjectsValues
                {
                    Folders = new List<ulong>(),
                    Items = new List<ulong>(),
                    Nations = new List<object>()
                };
                int lenghtName = reader.ReadByte();
                obj.Keys.Name = Encoding.GetEncoding(Settings.Encoding).GetString(reader.ReadBytes(lenghtName));
                obj.Keys.IsFolder = false;
                obj.Keys.Type = reader.ReadInt32();
                obj.Keys.ValueType = obj.Keys.Type == 9 ? reader.ReadInt32() : obj.Keys.Type;
                obj.Keys.NationsCount = obj.Keys.Type == 9 ? reader.ReadInt32() : 1;
                GetValuesByNations(reader, obj); //puxa valores
                _loadSaved.Add(obj.Id, obj);
            }
        }

        public static void PutEdit()
        {
            if (_loadSaved.Count == 0)
                return;
            int loaded = 0;
            foreach (Objects obj in _loadSaved.Values)
            {
                Objects actualObject = GetObjectById(obj.Id);
                if (actualObject != null)
                {
                    //object
                    actualObject.Type = obj.Type;
                    actualObject.Id = obj.Id;
                    actualObject.Offset = obj.Offset;
                    actualObject.Size = obj.Size;

                    //keys
                    actualObject.Keys.Name = obj.Keys.Name;
                    actualObject.Keys.IsFolder = obj.Keys.IsFolder;
                    actualObject.Keys.Type = obj.Keys.Type;
                    actualObject.Keys.ValueType = obj.Keys.ValueType;
                    actualObject.Keys.NationsCount = obj.Keys.NationsCount;

                    //nations
                    actualObject.Keys.Nations = obj.Keys.Nations;

                    loaded++;
                }
            }
            if (loaded > 0)
            {
                MessageBox.Show($"{loaded} edições carregadas.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private static string ReadUString(BinaryReader reader, int qty) => ReadUString(reader.ReadBytes(qty));
        private static string ReadUString(byte[] buffer)
        {
            string text = Encoding.GetEncoding(Settings.Encoding).GetString(buffer);
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
                if (i % 2 == 0)
                    s.Append(text[i]);
            string txt = s.ToString();
            s = null;
            return txt;
        }
    }
}
