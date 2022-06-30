namespace mqfDecryptor.Enums.Mission
{
    public class CardAwards
    {
        public int _id, _card, _insignia, _medal, _brooch, _exp, _gp;
        public bool Unusable()
        {
            return (_insignia == 0 && _medal == 0 && _brooch == 0 && _exp == 0 && _gp == 0);
        }
    }
}
