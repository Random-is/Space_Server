namespace Space_Server.model {
    public class ShipComponent {
        public int Tier { get; protected set; }
    }

    public class Gun_0 : ShipComponent, IFirstTier {
        public Gun_0() {
            Tier = 0;
        }
    }

    public class Gun_1 : ShipComponent, ISecondTier {
        public Gun_1() {
            Tier = 1;
        }
    }
    
    public class Gun_2 : ShipComponent, IThirdTier {
        public Gun_2() {
            Tier = 2;
        }
    }
    
    public class Gun_3 : ShipComponent, IFourthTier {
        public Gun_3() {
            Tier = 3;
        }
    }
    
    public class Gun_4 : ShipComponent, IFifthTier {
        public Gun_4() {
            Tier = 4;
        }
    }
}