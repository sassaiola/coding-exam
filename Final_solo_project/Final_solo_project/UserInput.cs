using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;


namespace Final_solo_project
{
    internal static class UserInput
    {
        private static KeyboardState _previousKeyboard;
        private static KeyboardState _currentKeyboard;

    

    public static void Update()
        {
            _previousKeyboard = _currentKeyboard;
            _currentKeyboard = Keyboard.GetState();
        }

        public static bool IsNewKeyPress(Keys key) 
        { 
            return _currentKeyboard.IsKeyDown(key) && _previousKeyboard.IsKeyUp(key); 
        }

        public static bool IsKeyDown(Keys key)
        {
            return _currentKeyboard.IsKeyDown(key);
        }





    } 
}
