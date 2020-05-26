namespace Bot
{
    public class VendorDetails
    {
        public string oui { get; set; }
        public bool isPrivate { get; set; }
        public string companyName { get; set; }
        public string companyAddress { get; set; }
        public string countryCode { get; set; }
    }

    public class BlockDetails
    {
        public bool blockFound { get; set; }
        public string borderLeft { get; set; }
        public string borderRight { get; set; }
        public int blockSize { get; set; }
        public string assignmentBlockSize { get; set; }
        public string dateCreated { get; set; }
        public string dateUpdated { get; set; }
    }

    public class MacAddressDetails
    {
        public string searchTerm { get; set; }
        public bool isValid { get; set; }
        public string virtualMachine { get; set; }
        public string[] applications { get; set; }
        public string transmissionType { get; set; }
        public string administrationType { get; set; }
        public string wiresharkNotes { get; set; }
        public string comment { get; set; }
    }

    public class Mac
    {
        public VendorDetails vendorDetails { get; set; }
        public BlockDetails blockDetails { get; set; }
        public MacAddressDetails macAddressDetails { get; set; }
    }
}