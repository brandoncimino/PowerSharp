namespace PowerSharp {
    public enum UniqueMode{
        CARDS,
        ART,
        PRINTS
    }

    public enum OrderBy {
        NAME,
        SET,
        RELEASED,
        RARITY,
        COLOR,
        USD,
        TIX,
        EUR,
        CMC,
        POWER,
        TOUGHNESS,
        EDHREC,
        ARTIST
    }

    public enum SortDirection {
        AUTO,
        ASC,
        DESC
    }

    public enum ImageVersion {
        LARGE,
        SMALL,
        NORMAL,
        PNG,
        ART_CROP,
        BORDER_CROP
    }
}