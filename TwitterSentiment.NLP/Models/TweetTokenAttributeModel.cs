namespace TwitterSentiment.NLP.Models
{
    public class TweetTokenAttributeModel
    {
        public int TweetId { get; set; }
        public int TokenIndex { get; set; }
        public string Token { get; set; }
        public string TokenStem { get; set; }
        public string POSTag { get; set; }
        public string DefinitiveTags { get; set; }
        public int RelatedToIndex { get; set; }
        public string TokenRelation { get; set; }
        public int IsNegated { get; set; }
        //public string TokenEng { get; set; }
        //public decimal? SenticNetPleasantness { get; set; }
        //public decimal? SenticNetAttention { get; set; }
        //public decimal? SenticNetSensitivity { get; set; }
        //public decimal? SenticNetAptitude { get; set; }
        //public string SenticNetPrimaryMood { get; set; }
        //public string SenticNetSecondaryMood { get; set; }
        public string SenticNetPolarityLabel { get; set; }
        public decimal? SenticNetPolarity { get; set; }
        //public string SenticNetSemantics1 { get; set; }
        //public string SenticNetSemantics2 { get; set; }
        //public string SenticNetSemantics3 { get; set; }
        //public string SenticNetSemantics4 { get; set; }
        //public string SenticNetSemantics5 { get; set; }
        public int IsDomainSpecific { get; set; }
        //public string SentiTurkNetSynonyms { get; set; }
        //public string SentiTurkNetGloss { get; set; }
        public string SentiTurkNetPolarityLabel { get; set; }
        //public string SentiTurkNetPOSTag { get; set; }
        public decimal? SentiTurkNetNegPolarity { get; set; }
        public decimal? SentiTurkNetObjPolarity { get; set; }
        public decimal? SentiTurkNetPosPolarity { get; set; }
        public int IsTurkish { get; set; }
        public string NamedEntity { get; set; }
    }
}
