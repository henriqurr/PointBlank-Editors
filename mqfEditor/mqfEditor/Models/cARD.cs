using mqfDecryptor.Enums.Mission;
using mqfDecryptor.Enums.Weapon;

namespace mqfDecryptor.Models
{
    public class Card
    {
        public ClassType _weaponReq;
        public MISSION_TYPE _missionType;
        public int _missionId, _mapId, _weaponReqId, _missionLimit, _missionBasicId, _cardBasicId,
            _arrayIdx, _flag;
        public Card(int cardBasicId, int missionBasicId)
        {
            _cardBasicId = cardBasicId;
            _missionBasicId = missionBasicId;
            _arrayIdx = (_cardBasicId * 4) + _missionBasicId;
            _flag = (15 << 4 * _missionBasicId);
        }
    }
}
