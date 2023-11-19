using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_task_manager
{
    internal class PasswordHiding //Клас, який приховує пароль під символами.
    {
        private string originalPasswordLine;

        private int previouseLengthPasswordLine;

        public string Points //Точки, за якими захований пароль.
        {
            get;
            private set;
        }

        public string OriginalPasswordLine //Оригінальний пароль.
        {
            get 
            {
                return originalPasswordLine;
            }
        }


        public void HidePasswordLine(string textBoxText) //Метод, що ховає пароль.
        {
            char[] symbolsArray = textBoxText.ToCharArray();
            int symbolsArrayLength = symbolsArray.Length;

            if (symbolsArrayLength < previouseLengthPasswordLine)
            {
                Points = "";
                originalPasswordLine = "";
                previouseLengthPasswordLine = 0;
            }
            else if (previouseLengthPasswordLine < 1)
            {
                originalPasswordLine = textBoxText;

                previouseLengthPasswordLine = symbolsArrayLength;

                Points = new string(Constants.BoldPoint, symbolsArrayLength);
            }
            else
            {
                if (symbolsArrayLength < 1)
                {
                    originalPasswordLine = "";
                }
                else
                {
                    char lastIndexSymbolsArray = symbolsArray[symbolsArrayLength - 1];
                    char crypticSymbol = Constants.BoldPoint;

                    if (lastIndexSymbolsArray != crypticSymbol)
                    {
                        originalPasswordLine += symbolsArray[symbolsArrayLength - 1];

                        symbolsArray[symbolsArrayLength - 1] = crypticSymbol;

                        Points = new string(symbolsArray);
                    }
                }

                previouseLengthPasswordLine = symbolsArray.Length;
            }
        }
    }
}
