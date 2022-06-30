using System.Collections.Generic;
using System.Windows.Forms;

namespace PropertyEditor.Models
{
    public class Objects
    {
        public int Type { get; set; }
        public ulong Id { get; set; }
        public ulong Offset { get; set; }
        public ulong NewOffset { get; set; }
        public ulong Size { get; set; }
        public ObjectsValues Keys { get; set; }
    }

    public class ObjectsValues
    {
        public string Name { get; set; }
        public int Type { get; set; } // tipo do item (Weapon == 9, ...)
        public int ValueType { get; set; } // tipo do valor (INT32, REAL32, STRING, ...)
        public int NationsCount { get; set; } // quantidade de vezes q vai adicionar o valor na nations
        public List<object> Nations { get; set; } // se for um item registra os valores dele
        public bool IsRegistryRoot { get; set; } // pasta inicial
        public bool IsFolder { get; set; } // pastas (TRN3)
        public List<ulong> Folders { get; set; } // se for uma pasta puxa todos os files de dentro da pasta
        public List<ulong> Items { get; set; } // puxa todos os items
    }
}
