// Keep these lines for a best effort IntelliSense of Visual Studio 2017 and higher.
/// <reference path="../../Packages/Beckhoff.TwinCAT.HMI.Framework.12.746.0/runtimes/native1.12-tchmi/TcHmi.d.ts" />
(function (/** @type {globalThis.TcHmi} */ TcHmi) {
    var Functions;
    (function (/** @type {globalThis.TcHmi.Functions} */ Functions) {
        var TcHmiProject1;
        (function (TcHmiProject1) {
            function GetMousePositionInPictureX(par1) {
                // läs in bildrutans x y storlek
                // 

                //imageWidth = 1000;
                //imageHeight = 1000;

                hmiWidth = 501;
                hmiHeight = 335;

                // get mouse pos in hmi
                var x = event.clientX - hmiWidth + hmiWidth / 2 - 18;
                x = Math.floor(x);
                //x = event.clientX * x;

                return x;
            }
            TcHmiProject1.GetMousePositionInPictureX = GetMousePositionInPictureX;
        })(TcHmiProject1 = Functions.TcHmiProject1 || (Functions.TcHmiProject1 = {}));
        Functions.registerFunctionEx('GetMousePositionInPictureX', 'TcHmi.Functions.TcHmiProject1', TcHmiProject1.GetMousePositionInPictureX);
    })(Functions = TcHmi.Functions || (TcHmi.Functions = {}));
})(TcHmi);