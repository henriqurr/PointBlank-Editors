using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mqfDecryptor
{
    public class Utils
    {
        public static int GetItemCategory(int id)
        {
            int value = getIdStatics(id, 1);
            if (value >= 1 && value <= 9) return 1;
            else if (value >= 10 && value <= 11) return 2;
            else if (value >= 12 && value <= 19) return 3;
            else MessageBox.Show("[Categoria INVÁLIDA] " + id);
            return 0;
        }

        public static int getIdStatics(int id, int type)
        {
            if (type == 1)
                return id / 100000000; //primeiros valores - classtype
            else if (type == 2)
                return (id % 100000000) / 1000000; //usage
            else if (type == 3)
                return (id % 1000000) / 1000; //valores do meio - type
            else if (type == 4)
                return id % 1000; //ultimos 3 valores - number
            return 0;
        }
    }
}
