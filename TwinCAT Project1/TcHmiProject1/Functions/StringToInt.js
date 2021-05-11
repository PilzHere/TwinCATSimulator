// Keep these lines for a best effort IntelliSense of Visual Studio 2017 and higher.
/// <reference path="../../Packages/Beckhoff.TwinCAT.HMI.Framework.12.746.0/runtimes/native1.12-tchmi/TcHmi.d.ts" />
(function (/** @type {globalThis.TcHmi} */ TcHmi) {
    var Functions;
    (function (/** @type {globalThis.TcHmi.Functions} */ Functions) {
        var TcHmiProject1;
        (function (TcHmiProject1) {
            function StringToInt(par1, string1) {
                var regExp = /[a-zA-Z]/g // Contains chars.

                if (regExp.test(string1)) {
                    return 0
                } else {
                    var stringParsedToInt = parseInt(string1)

                    return stringParsedToInt
                }    
            }
            TcHmiProject1.StringToInt = StringToInt;
        })(TcHmiProject1 = Functions.TcHmiProject1 || (Functions.TcHmiProject1 = {}));
        Functions.registerFunctionEx('StringToInt', 'TcHmi.Functions.TcHmiProject1', TcHmiProject1.StringToInt);
    })(Functions = TcHmi.Functions || (TcHmi.Functions = {}));
})(TcHmi);