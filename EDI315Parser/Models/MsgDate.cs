using System;

namespace EDI315Parser.Models
{
    public class MsgData
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string PartitionKey { get; set; } = "EDI315Data";
        public ISA isaSegment { get; set; }
        public GS gsSegment { get; set; }
        public ST stSegment { get; set; }
        public B4 b4Segment { get; set; }
        public N9 n9Segment { get; set; }
        public Q2 q2Segment { get; set; }
        public SG sgSegment { get; set; }
        public R4 r4Segment { get; set; }
        public SE seSegment { get; set; }
        public GE geSegment { get; set; }
        public IEA ieaSegment { get; set; }
    }
}
