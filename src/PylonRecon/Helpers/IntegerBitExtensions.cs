namespace PylonRecon.Helpers;

public static class IntegerBitExtensions
{
    private static readonly uint[] BaseBit = 
    {
        0x00000001, 0x00000002, 0x00000004, 0x00000008,
        0x00000010, 0x00000020, 0x00000040, 0x00000080,
        0x00000100, 0x00000200, 0x00000400, 0x00000800,
        0x00001000, 0x00002000, 0x00004000, 0x00008000,
        0x00010000, 0x00020000, 0x00040000, 0x00080000,
        0x00100000, 0x00200000, 0x00400000, 0x00800000,
        0x01000000, 0x02000000, 0x04000000, 0x08000000,
        0x10000000, 0x20000000, 0x40000000, 0x80000000,
    };

    private static readonly uint[] BaseBitReverse =
    {
        0xfffffffe, 0xfffffffd, 0xfffffffb, 0xfffffff7,
        0xffffffef, 0xffffffdf, 0xffffffbf, 0xffffff7f,
        0xfffffeff, 0xfffffdff, 0xfffffbff, 0xfffff7ff,
        0xffffefff, 0xffffdfff, 0xffffbfff, 0xffff7fff,
        0xfffeffff, 0xfffdffff, 0xfffbffff, 0xfff7ffff,
        0xffefffff, 0xffdfffff, 0xffbfffff, 0xff7fffff,
        0xfeffffff, 0xfdffffff, 0xfbffffff, 0xf7ffffff,
        0xefffffff, 0xdfffffff, 0xbfffffff, 0x7fffffff,
    };

    public static bool GetBit(this uint integer, int bit) => (integer & BaseBit[bit]) == BaseBit[bit];

    public static uint ModifyBit(this uint integer, int bit, bool target) =>
        target ? integer | BaseBit[bit] : integer & BaseBitReverse[bit];
}