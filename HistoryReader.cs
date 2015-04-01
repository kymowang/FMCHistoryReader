using System;
using System.Collections.Generic;
using System.Text;

namespace FMCReadHistory
{
    using System.Collections;
    using System.IO;
    using System.Windows.Forms;

    using log4net;

    class HistoryReader
    {
        public static void Run(string filePath)
        {
            ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            using (BinaryReader br = new BinaryReader(File.OpenRead(filePath)))
            {


                long pos = 0;
                int length = (int)br.BaseStream.Length;
                int block = 0x10;
                int index = 1;
                using (StreamWriter sw = new StreamWriter(filePath + "_head.csv", false, Encoding.Default))
                using (StreamWriter swHeater = new StreamWriter(filePath + "_heater.csv", false, Encoding.Default))
                using (StreamWriter swController = new StreamWriter(filePath + "_controller.csv", false, Encoding.Default))
                {
                    sw.WriteLine(@",数据标识,控制器总数目,真实控制器总数目,启用控制器总数目,热量表总数目,数据类型,分摊区段总热量,时刻");
                    swHeater.WriteLine(@",序号,数据标识,热量表型号,数据属性,年,月,日,时,热量表状态第1字节,累积热量,累积流量,热功率,流速,进水温度,回水温度,热量底数,区段热量,底数有效标志,满足分摊条件,热量表序号,操作符,分摊刚启动标志,热量表状态");
                    swController.WriteLine(@",序号,数据标识,所属区域管理器CID,控制器高位地址,控制器低位地址,控制器属性,控制器状态,房间面积,数据属性,存储记录的时刻,自供暖起始时累计运行时间,自供暖起始时累计开启时间,区段累计运行时间,区段累计开启时间,区段实际开度,房间实时温度,区段房间平均温度,用户设定温度, 阀门,	速热,强制开度,强制设定温度,遥控器电池状态,控制器工作状态,  遥控器显示热量,控制器启停,阀门通断状态, 区段分摊热量,总热量,区段修正开度,射频信号强度");

                    while (pos < length - 1)
                    {
                        //sw.WriteLine("<h1>"+index+"</h1><hr/>");
                        br.ReadBytes((int)block);
                        log.Info(br.BaseStream.Position.ToString("X") + " : " + length + " : " + (double)br.BaseStream.Position / length * 100 + "%");
                        sw.WriteLine("=====,批次:," + index + ",=====");
                        DataFlag df = (DataFlag)br.ReadByte();
                        sw.Write("," + df);
                        sw.Write("," + br.ReadInt16());
                        sw.Write("," + br.ReadInt16());
                        int controllerCount = br.ReadInt16();
                        sw.Write("," + controllerCount);
                        int heaterCount = br.ReadByte();
                        sw.Write("," + heaterCount);
                        sw.Write("," + (br.ReadByte() == 0 ? "运行数据" : "分摊数据") + "</td>");
                        sw.Write("," + br.ReadInt32());
                        sw.Write("," + ReadTime(br));
                        br.ReadBytes(31);
                        sw.WriteLine();
                        #region comment1
                        /*
                        Console.WriteLine(br.ReadByte()); //1	数据标识	0=头 1=热量表 2=控制器	本例=0
                        Console.WriteLine(br.ReadInt16()); //2	控制器总数目	整型，第1路和第2路控制器总数	
                        Console.WriteLine(br.ReadInt16()); //2	真实控制器总数目	整型，第1路和第2路真实控制器总数	
                        int controlCount = br.ReadInt16();
                        Console.WriteLine(controlCount); ///2	启用控制器总数目	整型，第1路和第2路启用控制器总数
                        int heatCount = br.ReadByte();
                        Console.WriteLine(heatCount); ///1	热量表总数目	单字节	
                        Console.WriteLine(br.ReadByte()); ///1	数据类型	0：运行数据     1：分摊数据	单字节
                        Console.WriteLine(br.ReadInt32()); //4	分摊区段总热量	单位：Wh	4字节长整型
                        //Console.WriteLine(br.ReadInt32());//4	时刻	4字节  年/月/日/时	运行数据读取时刻或分摊开始时刻
                        Console.WriteLine(br.ReadByte());
                        Console.WriteLine(br.ReadByte());
                        Console.WriteLine(br.ReadByte());
                        Console.WriteLine(br.ReadByte());
                        Console.WriteLine(br.ReadBytes(31)); //31	冗余	
                        */
                        #endregion

                        swHeater.WriteLine("=====,批次:," + (index) + ",=====");

                        for (int i = 0; i < heaterCount; i++)
                        {
                            log.Info("heater: " + i + " : " + br.BaseStream.Position.ToString("X"));
                            swHeater.Write("," + (i + 1));
                            swHeater.Write("," + br.ReadByte());
                            swHeater.Write("," + br.ReadByte());
                            swHeater.Write("," + (br.ReadByte() == 0 ? "无效" : "有效"));
                            swHeater.Write("," + br.ReadByte());
                            swHeater.Write("," + br.ReadByte());
                            swHeater.Write("," + br.ReadByte());
                            swHeater.Write("," + br.ReadByte());
                            swHeater.Write("," + br.ReadByte());
                            swHeater.Write("," + br.ReadInt32());
                            swHeater.Write("," + br.ReadInt32());
                            swHeater.Write("," + br.ReadInt32());
                            swHeater.Write("," + br.ReadInt32());
                            swHeater.Write("," + br.ReadInt16());
                            swHeater.Write("," + br.ReadInt16());
                            swHeater.Write("," + br.ReadInt32());
                            swHeater.Write("," + br.ReadInt32());
                            swHeater.Write("," + br.ReadByte());
                            swHeater.Write("," + br.ReadByte());
                            swHeater.Write("," + br.ReadByte());
                            swHeater.Write("," + br.ReadByte());
                            swHeater.Write("," + br.ReadByte());
                            swHeater.Write("," + br.ReadByte());
                            br.ReadBytes(6);

                            swHeater.WriteLine();
                            #region comment2
                            /*
                            Console.WriteLine("======热表数据=====" + i);

                            Console.WriteLine("数据标识: \t" + br.ReadByte());
                            Console.WriteLine("热量表型号: \t" + br.ReadByte());
                            Console.WriteLine("数据属性: \t" + br.ReadByte());
                            Console.WriteLine("年: \t" + br.ReadByte());
                            Console.WriteLine("月: \t" + br.ReadByte());
                            Console.WriteLine("日: \t" + br.ReadByte());
                            Console.WriteLine("时: \t" + br.ReadByte());
                            Console.WriteLine("热量表状态第1字节: \t" + br.ReadByte());
                            Console.WriteLine("累积热量: \t" + br.ReadInt32());
                            Console.WriteLine("累积流量: \t" + br.ReadInt32());
                            Console.WriteLine("热功率: \t" + br.ReadInt32());
                            Console.WriteLine("流速: \t" + br.ReadInt32());
                            Console.WriteLine("进水温度: \t" + br.ReadInt16());
                            Console.WriteLine("回水温度: \t" + br.ReadInt16());
                            Console.WriteLine("热量底数: \t" + br.ReadInt32());
                            Console.WriteLine("区段热量: \t" + br.ReadInt32());
                            Console.WriteLine("底数有效标志: \t" + br.ReadByte());
                            Console.WriteLine("满足分摊条件: \t" + br.ReadByte());
                            Console.WriteLine("热量表序号: \t" + br.ReadByte());
                            Console.WriteLine("操作符: \t" + br.ReadByte());
                            Console.WriteLine("分摊刚启动标志: \t" + br.ReadByte());
                            Console.WriteLine("热量表状态第2字节: \t" + br.ReadByte());

                            Console.WriteLine("冗余: \t" + br.ReadBytes(6));*/
                            #endregion
                        }

                        #region header
                        swController.WriteLine("=====,批次:," + (index++) + ",=====");
                        #endregion
                        //控制器
                        for (int i = 0; i < controllerCount; i++)
                        {
                            log.Info("controller: " + i + " : " + br.BaseStream.Position.ToString("X"));
                            swController.Write("," + (i + 1));
                            swController.Write("," + br.ReadByte());
                            string x = string.Empty;
                            for (int j = 0; j < 6; j++) x += br.ReadByte().ToString("X2");
                            byte high = br.ReadByte();
                            byte low = br.ReadByte();
                            swController.Write("," + x + high.ToString("X2") + low.ToString("X2"));
                            swController.Write("," + high);
                            swController.Write("," + low);
                            byte cProperty = br.ReadByte();
                            byte cStatus = br.ReadByte();
                            swController.Write("," + (cProperty == 0 ? "虚拟" : "真实"));
                            swController.Write("," + (cStatus == 0 ? "停用" : "启用"));
                            swController.Write("," + br.ReadInt16());
                            swController.Write("," + br.ReadByte());
                            swController.Write("," + ReadTime(br));
                            swController.Write("," + br.ReadInt16());
                            swController.Write("," + br.ReadInt16());
                            swController.Write("," + br.ReadInt16());
                            swController.Write("," + br.ReadInt16());
                            swController.Write("," + br.ReadInt16());
                            swController.Write("," + br.ReadInt16());
                            swController.Write("," + br.ReadInt16());
                            swController.Write("," + br.ReadInt16());

                            byte state1 = br.ReadByte();
                            byte state2 = br.ReadByte();
                            var state1Arr = Convert.ToString(state1, 2);
                            var state2Arr = Convert.ToString(state2, 2).PadLeft(8,'0');
                            //var state1Arr2 = new BitArray(new byte[] { state1 });
                            state1Arr = state1Arr.PadLeft(8, '0');
                            var statex1 = state1Arr.Substring(4, 2);
                            string statex11 = "未知";
                            if (statex1 == "00") statex11 = "欠压";
                            else if (statex1 == "01") statex11 = "正常";
                            var statex2 = state1Arr.Substring(6, 2);
                            string statex21 = "xx";
                            if (statex2 == "00") statex21 = "未对码";
                            else if (statex2 == "10") statex21 = "未连接";
                            else if (statex2 == "11") statex21 = "正常";


                            swController.Write("," + (int.Parse(state1Arr.Substring(0,1))==0?"故障":"正常"));
                            swController.Write("," + (int.Parse(state1Arr.Substring(1, 1)) == 0 ? "不速热" : "速热"));
                            swController.Write("," + (int.Parse(state1Arr.Substring(2, 1)) == 0 ? "不强制" : "强制"));
                            swController.Write("," + (int.Parse(state1Arr.Substring(3, 1)) == 0 ? "不强制" : "强制"));
                            swController.Write("," + statex11);
                            swController.Write("," + statex21);

                            swController.Write("," + (int.Parse(state1Arr.Substring(5, 1)) == 0 ? "显示" : "不显示"));
                            swController.Write("," + (int.Parse(state1Arr.Substring(6, 1)) == 0 ? "停止" : "启动"));
                            swController.Write("," + (int.Parse(state1Arr.Substring(7, 1)) == 0 ? "断" : "通"));

                            swController.Write("," + br.ReadInt32());
                            swController.Write("," + br.ReadInt32());
                            swController.Write("," + br.ReadInt16());
                            swController.Write("," + br.ReadSByte());
                            swController.WriteLine();
                            br.ReadByte();
                            #region comment3
                            //Console.WriteLine();
                            //Console.WriteLine("======控制器=====");
                            //Console.WriteLine("数据标识: \t" + br.ReadByte());
                            //Console.Write("所属区域管理器CID: \t");
                            //for (int j = 0; j < 6; j++) Console.Write(br.ReadByte());
                            //Console.WriteLine();
                            //Console.WriteLine("控制器高位地址: \t" + br.ReadByte());
                            //Console.WriteLine("控制器低位地址: \t" + br.ReadByte());
                            //Console.WriteLine("控制器属性: \t" + br.ReadByte());
                            //Console.WriteLine("控制器状态: \t" + br.ReadByte());
                            //Console.WriteLine("房间面积: \t" + br.ReadInt16());
                            //Console.WriteLine("数据属性: \t" + br.ReadByte());
                            //Console.WriteLine("存储记录的时刻: \t");
                            //for (int j = 0; j < 4; j++) Console.Write(br.ReadByte() + "/");
                            //Console.WriteLine();
                            //Console.WriteLine("自供暖起始时累计运行时间: \t" + br.ReadInt16());
                            //Console.WriteLine("自供暖起始时累计开启时间: \t" + br.ReadInt16());
                            //Console.WriteLine("区段累计运行时间: \t" + br.ReadInt16());
                            //Console.WriteLine("区段累计开启时间: \t" + br.ReadInt16());
                            //Console.WriteLine("区段实际开度: \t" + br.ReadInt16());
                            //Console.WriteLine("房间实时温度: \t" + br.ReadInt16());
                            //Console.WriteLine("区段房间平均温度: \t" + br.ReadInt16());
                            //Console.WriteLine("用户设定温度: \t" + br.ReadInt16());
                            //Console.WriteLine("系统工作状态1: \t" + br.ReadByte());
                            //Console.WriteLine("系统工作状态2: \t" + br.ReadByte());
                            //Console.WriteLine("区段分摊热量: \t" + br.ReadInt32());
                            //Console.WriteLine("总热量: \t" + br.ReadInt32());
                            //Console.WriteLine("区段修正开度: \t" + br.ReadInt16());
                            //Console.WriteLine("射频信号强度: \t" + br.ReadByte());
                            //Console.WriteLine("冗余: \t" + br.ReadByte());
                            //Console.ReadLine();
                            #endregion
                        }

                        br.ReadBytes(block);
                        pos = br.BaseStream.Position;
                    }
                }
            }
        }

        static string ReadTime(BinaryReader br)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(br.ReadByte().ToString());
            sb.Append("-");
            sb.Append(br.ReadByte().ToString());
            sb.Append("-");
            sb.Append(br.ReadByte().ToString());
            sb.Append(" ");
            sb.Append(br.ReadByte().ToString());
            sb.Append("：00");
            return sb.ToString();
        }
    }
}
