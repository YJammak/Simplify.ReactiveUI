using Splat;

namespace Simplify.ReactiveUI.Domain;

[SplatRegister([typeof(IViewModel)])]
[SplatRegisterConstant]
[SplatRegisterViewModel(typeof(object))]
public class ViewModel1 : IViewModel
{
    private void Test()
    {
        Locator.CurrentMutable.RegisterAllSimplifyReactiveUIDomain();
    }
}
