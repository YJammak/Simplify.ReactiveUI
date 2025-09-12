using Splat;

namespace Simplify.ReactiveUI.Domain;

[SplatRegister([typeof(IViewModel)])]
[SplatRegisterConstant]
[SplatRegisterLazySingleton]
public class ViewModel1 : IViewModel
{
    private void Test()
    {
        Locator.CurrentMutable.RegisterAll();
    }
}
