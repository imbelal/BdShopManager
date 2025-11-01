namespace Domain.Enums
{
    public enum ProductStatus
    {
        Active = 1,
        InActive = 2
    }

    public enum ProductUnit
    {
        Box = 0,
        Piece = 1,
        SquareFeet = 2,
        Kilogram = 3,
        Gram = 4,
        Liter = 5,
        Milliliter = 6,
        Meter = 7,
        Centimeter = 8,
        Inch = 9,
        Yard = 10,
        Ton = 11,
        Pack = 12,
        Dozen = 13,
        Pair = 14,
        Roll = 15,
        Bundle = 16,
        Carton = 17,
        Bag = 18,
        Set = 19,
        Barrel = 20,
        Gallon = 21,
        Can = 22,
        Tube = 23,
        Packet = 24,
        Unit = 25
    }

    public enum UserRoleType
    {
        Admin = 0,
        Moderator = 1,
        ReadOnlyUser = 2
    }

    public enum SalesStatus
    {
        Pending = 0,        // No payment made yet
        PartiallyPaid = 1,  // Some payment made but not full
        Paid = 2,           // Full payment received
        Cancelled = 3       // Order cancelled
    }

    public enum StockTransactionType
    {
        IN = 0,   // Stock coming in (increase)
        OUT = 1   // Stock going out (decrease)
    }

    public enum StockReferenceType
    {
        Purchase = 0,
        Sale = 1,
        SalesReturn = 2,
        Adjustment = 3
    }

}
