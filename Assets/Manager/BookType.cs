public enum BookType
{
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten
}

[System.Serializable]
public class BookPlacement
{
    public BookType bookType;
    public int row;
    public int col;
}