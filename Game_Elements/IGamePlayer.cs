using Game_Elements.ship;
using Game_Elements.ship.ship_part;
using Game_Elements.utility;

namespace Game_Elements {
    public interface IGamePlayer {
        void Reset();
        void AddXp(int xp);
        void SetXp(int xp);
        void ChangeHp(int hp);
        void SetHp(int hp);
        void ChangeMoney(int money);
        void SetMoney(int money);
        IntVector2 AddShipToFreePosition(Ship ship);
        void AddShipToPosition(Ship ship, IntVector2 position);
        Ship ShipReposition(Ship ship, int newY, int newX);
        ShipPart BuyShipPart(int shopItemIndex);
        int AddBagItem(ShipPart shipPart);
        ShipPart SellBagItem(int bagIndex);
        ShipPart BagItemReposition(int oldItemIndex, int newItemIndex);
        ShipPart AddShipPartToShipAndSell(Ship ship, int bagIndex);
        void SetShop(ShipPart[] shop);
    }
}