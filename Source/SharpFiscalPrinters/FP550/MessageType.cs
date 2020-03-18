namespace SharpFiscalPrinters.FP550
{
    internal enum MessageType
    {
        [ProgramFormat("")]
        [PrinterFormat("")]
        [MessageNumber(33)]
        M33,

        [ProgramFormat("T")]
        [PrinterFormat("")]
        [MessageNumber(35)]
        M35,

        [ProgramFormat("")]
        [PrinterFormat("W,T")]
        [MessageNumber(38)]
        M38,

        [ProgramFormat("")]
        [PrinterFormat("W")]
        [MessageNumber(39)]
        M39,

        [ProgramFormat("T")]
        [PrinterFormat("")]
        [MessageNumber(42)]
        M42,

        [ProgramFormat("TT")]
        [PrinterFormat("T")]
        [MessageNumber(43)]
        M43,

        [ProgramFormat("")]
        [PrinterFormat("")]
        [MessageNumber(44)]
        M44,

        [ProgramFormat("B")]
        [PrinterFormat("")]
        [MessageNumber(44)]
        M44L,

        [ProgramFormat("B,B")]
        [PrinterFormat("")]
        [MessageNumber(44)]
        M44LO,

        [ProgramFormat("")]
        [PrinterFormat("T")]
        [MessageNumber(45)]
        M45,

        [ProgramFormat("T")]
        [PrinterFormat("")]
        [MessageNumber(47)]
        M47,

        [ProgramFormat("T;T,B")]
        [PrinterFormat("W,W")]
        [MessageNumber(48)]
        M48,

        [ProgramFormat("")]
        [PrinterFormat("")]
        [MessageNumber(50)]
        M50,

        [ProgramFormat("T,T")]
        [PrinterFormat("")]
        [MessageNumber(50)]
        M50SE,

        [ProgramFormat("B")]
        [PrinterFormat("B,B,B,B,B,B,B,B,B,B")]
        [MessageNumber(51)]
        M51,

        [ProgramFormat("ST")]
        [PrinterFormat("")]
        [MessageNumber(52)]
        M52,

        [ProgramFormat("STT")]
        [PrinterFormat("")]
        [MessageNumber(52)]
        M52S,

        [ProgramFormat("ST*T")]
        [PrinterFormat("")]
        [MessageNumber(52)]
        M52Q,

        [ProgramFormat("STT*T")]
        [PrinterFormat("")]
        [MessageNumber(52)]
        M52SQ,

        [ProgramFormat("")]
        [PrinterFormat("TB")]
        [MessageNumber(53)]
        M53,

        [ProgramFormat("B")]
        [PrinterFormat("TB")]
        [MessageNumber(53)]
        M53A,

        [ProgramFormat("T")]
        [PrinterFormat("TB")]
        [MessageNumber(53)]
        M53P,

        [ProgramFormat("TB")]
        [PrinterFormat("TB")]
        [MessageNumber(53)]
        M53PA,

        [ProgramFormat("")]
        [PrinterFormat("W,W")]
        [MessageNumber(56)]
        M56,

        [ProgramFormat("ST")]
        [PrinterFormat("")]
        [MessageNumber(58)]
        M58,

        [ProgramFormat("STT")]
        [PrinterFormat("")]
        [MessageNumber(58)]
        M58S,

        [ProgramFormat("ST*T")]
        [PrinterFormat("")]
        [MessageNumber(58)]
        M58Q,

        [ProgramFormat("STT*T")]
        [PrinterFormat("")]
        [MessageNumber(58)]
        M58SQ,

        [ProgramFormat("B")]
        [PrinterFormat("")]
        [MessageNumber(60)]
        M60,

        [ProgramFormat("Q")]
        [PrinterFormat("")]
        [MessageNumber(61)]
        M61,

        [ProgramFormat("q")]
        [PrinterFormat("")]
        [MessageNumber(61)]
        M61S,

        [ProgramFormat("")]
        [PrinterFormat("B-B-B B:B:B")]
        [MessageNumber(62)]
        M62,

        [ProgramFormat("")]
        [PrinterFormat("T")]
        [MessageNumber(62)]
        M62T,

        [ProgramFormat("")]
        [PrinterFormat("")]
        [MessageNumber(63)]
        M63,

        [ProgramFormat("")]
        [PrinterFormat("W,B,B,B,B,B,B,B,B,B,T")]
        [MessageNumber(64)]
        M64,

        [ProgramFormat("")]
        [PrinterFormat("B,B,B,B,B,B,B,B,B,B")]
        [MessageNumber(65)]
        M65,

        [ProgramFormat("")]
        [PrinterFormat("B,B,B,B,B")]
        [MessageNumber(67)]
        M67,

        [ProgramFormat("")]
        [PrinterFormat("W,W")]
        [MessageNumber(68)]
        M68,

        [ProgramFormat("")]
        [PrinterFormat("W,B,B,B,B,B,B,B,B,B,B")]
        [MessageNumber(69)]
        M69,

        [ProgramFormat("B")]
        [PrinterFormat("W,B,B,B,B,B,B,B,B,B,B")]
        [MessageNumber(69)]
        M69O,

        [ProgramFormat("BN")]
        [PrinterFormat("W,B,B,B,B,B,B,B,B,B,B")]
        [MessageNumber(69)]
        M69ON,

        [ProgramFormat("BA")]
        [PrinterFormat("W,B,B,B,B,B,B,B,B,B,B")]
        [MessageNumber(69)]
        M69OA,

        [ProgramFormat("BNA")]
        [PrinterFormat("W,B,B,B,B,B,B,B,B,B,B")]
        [MessageNumber(69)]
        M69ONA,

        [ProgramFormat("")]
        [PrinterFormat("T,B,B,B")]
        [MessageNumber(70)]
        M70,

        [ProgramFormat("B")]
        [PrinterFormat("T,B,B,B")]
        [MessageNumber(70)]
        M70A,

        [ProgramFormat("")]
        [PrinterFormat("")]
        [MessageNumber(71)]
        M71,

        [ProgramFormat("T")]
        [PrinterFormat("T")]
        [MessageNumber(72)]
        M72,

        [ProgramFormat("B,B,T")]
        [PrinterFormat("")]
        [MessageNumber(73)]
        M73,

        [ProgramFormat("")]
        [PrinterFormat("TTTTTT")]
        [MessageNumber(74)]
        M74,

        [ProgramFormat("T")]
        [PrinterFormat("TTTTTT")]
        [MessageNumber(74)]
        M74O,

        [ProgramFormat("")]
        [PrinterFormat("T,W,B,B")]
        [MessageNumber(76)]
        M76,

        [ProgramFormat("T")]
        [PrinterFormat("T,W,B,B")]
        [MessageNumber(76)]
        M76O,

        [ProgramFormat("T,T")]
        [PrinterFormat("")]
        [MessageNumber(79)]
        M79,

        [ProgramFormat("")]
        [PrinterFormat("")]
        [MessageNumber(80)]
        M80,

        [ProgramFormat("")]
        [PrinterFormat("B,T,B,B,B,B,B,B,B,B,B")]
        [MessageNumber(83)]
        M83,

        [ProgramFormat("B,T,B,B,B,B,B,B,B,B,B")]
        [PrinterFormat("????")] // TODO: Unknown
        [MessageNumber(83)]
        M83DFX,

        [ProgramFormat("T")]
        [PrinterFormat("T,W")]
        [MessageNumber(89)]
        M89,

        [ProgramFormat("T")]
        [PrinterFormat("T,T,T,T,T,T")]
        [MessageNumber(90)]
        M90,

        [ProgramFormat("B,T")]
        [PrinterFormat("T,T")]
        [MessageNumber(91)]
        M91,

        [ProgramFormat("T")]
        [PrinterFormat("T")]
        [MessageNumber(92)]
        M92,

        [ProgramFormat("")]
        [PrinterFormat("B,B,B,B,B,B,B,B,B")]
        [MessageNumber(97)]
        M97,

        [ProgramFormat("T")]
        [PrinterFormat("T")]
        [MessageNumber(98)]
        M98,

        [ProgramFormat("")]
        [PrinterFormat("T,T")]
        [MessageNumber(99)]
        M99,

        [ProgramFormat("T")]
        [PrinterFormat("")]
        [MessageNumber(100)]
        M100,

        [ProgramFormat("T,T,T")]
        [PrinterFormat("")]
        [MessageNumber(101)]
        M101,

        [ProgramFormat("T,T,T")]
        [PrinterFormat("")]
        [MessageNumber(102)]
        M102,

        [ProgramFormat("")]
        [PrinterFormat("T,B,B,B,B,B,B,B,B,B")]
        [MessageNumber(103)]
        M103,

        [ProgramFormat("T,T")]
        [PrinterFormat("")]
        [MessageNumber(104)]
        M104,

        [ProgramFormat("")]
        [PrinterFormat("")]
        [MessageNumber(105)]
        M105,

        [ProgramFormat("")]
        [PrinterFormat("")]
        [MessageNumber(106)]
        M106,

        [ProgramFormat("B")]
        [PrinterFormat("")]
        [MessageNumber(106)]
        M106I,

        [ProgramFormat("PTB,B,T")]
        [PrinterFormat("T")]
        [MessageNumber(107)]
        M107P,

        [ProgramFormat("DA")]
        [PrinterFormat("T")]
        [MessageNumber(107)]
        M107DA,

        [ProgramFormat("DB")]
        [PrinterFormat("T")]
        [MessageNumber(107)]
        M107D,

        [ProgramFormat("RB")]
        [PrinterFormat("T,B,T,B,B,T")]
        [MessageNumber(107)]
        M107R,

        [ProgramFormat("CB,B")]
        [PrinterFormat("T")]
        [MessageNumber(107)]
        M107C,

        [ProgramFormat("F")]
        [PrinterFormat("T,B,T,B,B,T")]
        [MessageNumber(107)]
        M107F,

        [ProgramFormat("N")]
        [PrinterFormat("T,B,T,B,B,T")]
        [MessageNumber(107)]
        M107N,

        [ProgramFormat("f")]
        [PrinterFormat("T,B,T,B,B,T")]
        [MessageNumber(107)]
        M107f,

        [ProgramFormat("n")]
        [PrinterFormat("T,B,T,B,B,T")]
        [MessageNumber(107)]
        M107n,

        [ProgramFormat("X")]
        [PrinterFormat("B")]
        [MessageNumber(107)]
        M107X,

        [ProgramFormat("I")]
        [PrinterFormat("B,B,B")]
        [MessageNumber(107)]
        M107I,

        [ProgramFormat("")]
        [PrinterFormat("B,B,B,B,B")]
        [MessageNumber(110)]
        M110,

        [ProgramFormat("B")]
        [PrinterFormat("T")]
        [MessageNumber(111)]
        M111,

        [ProgramFormat("T")]
        [PrinterFormat("B,B;B,B;B,B;B,B;B,T;T")]
        [MessageNumber(112)]
        M112,

        [ProgramFormat("T")]
        [PrinterFormat("B,B;B,B;B,B;B,B;B,T")]
        [MessageNumber(112)]
        M112f,

        [ProgramFormat("")]
        [PrinterFormat("B")]
        [MessageNumber(113)]
        M113,

        [ProgramFormat("B")]
        [PrinterFormat("T,B,B,B,B,B,B,B,B")]
        [MessageNumber(114)]
        M114,

        [ProgramFormat("B,B")]
        [PrinterFormat("T,B,B,B,B,B,B,B,B")]
        [MessageNumber(114)]
        M114T,

        [ProgramFormat("B,B,B")]
        [PrinterFormat("T,B,B,B,B,B,B,B,B")]
        [MessageNumber(114)]
        M114TC,

        [ProgramFormat("B,T")]
        [PrinterFormat("")]
        [MessageNumber(115)]
        M115,

        [ProgramFormat("T,B")]
        [PrinterFormat("T")]
        [MessageNumber(116)]
        M116,

        [ProgramFormat("B")]
        [PrinterFormat("T")]
        [MessageNumber(117)]
        M117,

        ERROR_NAK,

        ERROR_SYN,
    }
}
