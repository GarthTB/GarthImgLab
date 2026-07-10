namespace GarthImgLab.Benchmarks;

public static class Helper {
    public static void MockMap(ref ushort r, ref ushort g, ref ushort b) {
        ushort r0 = r, g0 = g, b0 = b;
        unchecked {
            r = (ushort)(g0 * 31 + 1);
            g = (ushort)(b0 * 37 + 3);
            b = (ushort)(r0 * 41 + 5);
        }
    }
}
