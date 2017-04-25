using System;
using System.Collections.Generic;
using System.Text;

namespace Kachuwa.Web.API
{
    public class ApiResponseCodes
    {
        public enum Codes
        {
            User_Not_Registerd = 901,
            ModelValidationError = 904,
            LoginError = 902,
            SavingError = 903,
            UserAlreadyExist = 905,
            InvalidFileExtensions = 906,
            FileNotFound = 907,
            InvalidUser = 908
        }
    }
}
