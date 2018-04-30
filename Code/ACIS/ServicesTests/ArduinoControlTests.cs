using Data;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Tests
{
    [TestClass()]
    public class ArduinoControlTests
    {
        [TestMethod()]
        public void ReciveCommandTest()
        {
            //using (ShimsContext.Create())
            //{
            //    var expectedDevice = (int)Motors.X_AXIS_TOP;
            //    var expectedFunction = (int)ArduinoFunctions.MOVE_BACKWARD;
            //    var expectedDistance = 200;
            //    var expectedStatusCode = (int)Erros.SUCCESS;
            //    int actualDevice = -1;
            //    int actualFunction = -1;
            //    int actualDistance = -1;
            //    int actualStatuCode = -1;

            //    //Arrange
            //    System.IO.Ports.Fakes.ShimSerialPort.AllInstances.BytesToReadGet = (SerialPort serial) =>
            //    {
            //        return Constants.NUMBER_OF_BYTES_TO_RECEIVE;
            //    };
            //    System.IO.Ports.Fakes.ShimSerialPort.AllInstances.ReadByteArrayInt32Int32 = (SerialPort serial, byte[] buffer, int offset, int count) =>
            //    {

            //        buffer[0] = (byte)(expectedDevice | (expectedFunction << 3) );
            //        buffer[1] = (byte)expectedDistance;
            //        buffer[2] = (byte)expectedStatusCode;
            //        return 3;
            //    };


                ////Act 
                //var ac = new ArduinoControl();

                //ac.ReciveCommand(ref actualDevice, ref actualFunction, ref actualStatuCode, ref actualDistance);

                ////Assert
                //Assert.AreEqual(expectedDevice, actualDevice);
                //Assert.AreEqual(expectedFunction, actualFunction);
                //Assert.AreEqual(expectedDistance, actualDistance);
                //Assert.AreEqual(actualStatuCode, actualStatuCode);

            //}
        }
    }
}