// Keep these lines for a best effort IntelliSense of Visual Studio 2017 and higher.
/// <reference path="../../Packages/Beckhoff.TwinCAT.HMI.Framework.12.746.0/runtimes/native1.12-tchmi/TcHmi.d.ts" />
(function (/** @type {globalThis.TcHmi} */ TcHmi) {
    var Functions;
    (function (/** @type {globalThis.TcHmi.Functions} */ Functions) {
        var TcHmiProject1;
        (function (TcHmiProject1) {
            function GetCurrentPicture(chosenBatteryNumber) {

                var AAbatteryList = new Array("824", "825", "826", "827", "828", "829", "830", "831", "832", "833", "834", "835", "836", "837", "838", "839", "840", "841", "842", "843", "844", "845", "846", "847", "848", "849", "850", "851", "852", "853", "854", "855", "856")
                return "Images/AA/DSC05" + AAbatteryList[chosenBatteryNumber] + ".JPG"

                //var chosenPictureNumber = 824

                //var rand
                /*switch (chosenBatteryNumber) {
                    case 0:
                        return ""
                    case 1: // AA
                        var AAbatteryList = new Array("824", "825", "826", "827", "828", "829", "830", "831", "832", "833", "834", "835", "836", "837", "838", "839", "840", "841", "842", "843", "844", "845", "846", "847", "848", "849", "850", "851", "852", "853", "854", "855", "856")
                        //rand = Math.floor(Math.random() * AAbatteryList.length)
                        //return "Images/AA/DSC05" + AAbatteryList[rand] + ".JPG"

                        return "Images/AA/DSC05" + AAbatteryList[chosenPictureNumber] + ".JPG"

                        //return "Images/AA/DSC05" + chosenPictureNumber + ".JPG"

                    case 2: // AAA
                        var AAAbatteryList = new Array("793", "794", "795", "796", "797", "798", "799", "800", "801", "802", "803", "804", "805", "806", "807", "808", "809", "810", "811", "812", "813", "814", "815", "816", "817", "818", "819", "820", "821", "822", "823")
                        //rand = Math.floor(Math.random() * AAAbatteryList.length)
                        //return "Images/AAA/DSC05" + AAAbatteryList[rand] + ".JPG"

                        //return "Images/AAA/DSC05" + AAAbatteryList[chosenPictureNumber] + ".JPG"

                        return "Images/AAA/DSC05" + chosenPictureNumber + ".JPG"
                    default:
                        return ""
                }*/

                // function to return string of picture depending of int input
                /*
                lista
                returnpicture

                return "Images/AA/" + imagename

                var pathToCurrentPicture
                switch (par1) {
                    case 0:
                        pathToCurrentPicture = ""
                        switch (par2) {
                            case 0:
                                returnpicture = list[0]
                                break;
                        }
                        break
                    case 1:
                        pathToCurrentPicture = "Images/Beckhoff_Logo.svg"
                        break
                    default:
                        pathToCurrentPicture = ""
                        break;
                }

                return pathToCurrentPicture */
            }
            TcHmiProject1.GetCurrentPicture = GetCurrentPicture;
        })(TcHmiProject1 = Functions.TcHmiProject1 || (Functions.TcHmiProject1 = {}));
        Functions.registerFunctionEx('GetCurrentPicture', 'TcHmi.Functions.TcHmiProject1', TcHmiProject1.GetCurrentPicture);
    })(Functions = TcHmi.Functions || (TcHmi.Functions = {}));
})(TcHmi);