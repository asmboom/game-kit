using UnityEngine;

namespace Beetle23
{
    [System.Serializable]
    public class SerializableItem : IItem
    {
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public SerializableItem()
        {
            ID = GetUniqueCode();
        }

        // the below code is referenced from https://gist.github.com/tracend/8203090
        private string GetUniqueCode()
        {
            string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#";
            string ticks = System.DateTime.UtcNow.Ticks.ToString();
            var code = "";
            for (var i = 0; i < characters.Length; i += 2)
            {
                if ((i + 2) <= ticks.Length)
                {
                    var number = int.Parse(ticks.Substring(i, 2));
                    if (number > characters.Length - 1)
                    {
                        var one = double.Parse(number.ToString().Substring(0, 1));
                        var two = double.Parse(number.ToString().Substring(1, 1));
                        code += characters[System.Convert.ToInt32(one)];
                        code += characters[System.Convert.ToInt32(two)];
                    }
                    else
                    {
                        code += characters[number];
                    }
                }
            }
            return code;
        }

        [SerializeField]
        private string _id;

        [SerializeField]
        private string _name;
    }
}
