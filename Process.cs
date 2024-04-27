using Framework;
#pragma warning disable CS8618

class Process
{
    static void Main(string[] args)
    {
        Memory.init("RustClient.exe", "GameAssembly.dll");

        Methods.initMethods();
        Methods.changeTime(1f);
        Methods.changeFov(120f);
        Methods.infiniteJump();
    }
}