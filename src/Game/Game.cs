namespace ChessBoom.Game
{
    enum Variant {
        Standard,
        Chess960,
        Atomic,
        Horde
    }

    enum Player {
        White,
        Black
    }
    public class Game {
        private Variant m_variant;
        private Board m_board;

        public Game() {
            m_variant = Variant.Standard;
            m_board = GetInitialBoard(m_variant);
        }

        private Board GetInitialBoard(Variant variant) {
            // Note: Standard and Atomic use the default board. Chess960 and Horde use different initial configurations
            switch (variant) {
                case Variant.Chess960:
                break;
                case Variant.Horde:
                break;
                case Variant.Standard:
                case Variant.Atomic:
                default:
                break;
            }
        }
    }
}