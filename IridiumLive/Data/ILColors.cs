/*
 * 
 * https://idearmy.com/html-color-codes/
 * 
 *  lightsalmon #FFA07A
 *  salmon	#FA8072
 *  darksalmon	#E9967A
 *  lightcoral	#F08080
 *  indianred	#CD5C5C
 *  crimson	#DC143C
 *  firebrick	#B22222
 *  red	#FF0000
 * 
 * 	powderblue	#B0E0E6
lightblue	#ADD8E6
lightskyblue	#87CEFA
skyblue	#87CEEB
deepskyblue	#00BFFF
lightsteelblue	#B0C4DE
dodgerblue	#1E90FF
cornflowerblue	#6495ED
steelblue	#4682B4
royalblue	#4169E1
blue	#0000FF
 * */

namespace IridiumLive.Data
{
    public static class ILColors
    {
        public static string ILColor(double altitude, double quality)
        {
            if ((int)altitude < 100)
            {
                switch ((int)quality)
                {
                    case 100:
                        return "#0000FF";
                    case 99:
                        return "#4169E1";
                    case 98:
                        return "#4682B4";
                    case 97:
                        return "#6495ED";
                    case 96:
                        return "#1E90FF";
                    case 95:
                        return "#B0C4DE";
                    case 94:
                        return "#00BFFF";

                    default:
                        return "#87CEEB";
                }
            }
            else
            {
                switch ((int)quality)
                {
                    case 100:
                        return "#B22222";
                    case 99:
                        return "#FF0000";
                    case 98:
                        return "#DC143C";
                    case 97:
                        return "#CD5C5C";
                    case 96:
                        return "#F08080";
                    case 95:
                        return "#E9967A";
                    case 94:
                        return "#FA8072";

                    default:
                        return "#FFA07A";
                }
            }
        }
    }
}