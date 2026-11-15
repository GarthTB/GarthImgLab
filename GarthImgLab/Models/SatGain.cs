namespace GarthImgLab.Models;

enum SatGainMode: byte {
    CieLCh,
    Hsv,
    Hsl,
    JzCzhz,
    OkLCh,
    OkLrCh,
    OkLChPlus,
    SCam,
    AcesJmh
}

sealed class SatGain(SatGainMode mode, double gain) {
    double Gain(double sat) {
        var t = 1 - sat;
        var tp = Math.Pow(t, 2.8);
        return sat + 2 * gain * t * (1 - tp) / (2 - tp);
    }
}
