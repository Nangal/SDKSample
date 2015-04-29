using Qlik.Engine;
using Qlik.Sense.Client;
using Qlik.Sense.Client.Visualizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDKSample
{
    class Program
    {
        // Prepare your Qlik Sense Desktop
        // add a CleanCtrlQ with the following script
        
//Characters:
//Load Chr(RecNo()+Ord('A')-1) as Alpha, RecNo() as Num autogenerate 26;
 
//ASCII:
//Load 
// if(RecNo()>=65 and RecNo()<=90,RecNo()-64) as Num,
// Chr(RecNo()) as AsciiAlpha, 
// RecNo() as AsciiNum
//autogenerate 255
// Where (RecNo()>=32 and RecNo()<=126) or RecNo()>=160 ;
 


//Transactions:
//Load
// TransLineID, 
// TransID,
// mod(TransID,26)+1 as Num,
// Pick(Ceil(3*Rand1),'A','B','C') as Dim1,
// Pick(Ceil(6*Rand1),'a','b','c','d','e','f') as Dim2,
// Pick(Ceil(3*Rand()),'X','Y','Z') as Dim3,
// Round(1000*Rand()*Rand()*Rand1) as Expression1,
// Round(  10*Rand()*Rand()*Rand1) as Expression2,
// Round(Rand()*Rand1,0.00001) as Expression3;
//Load 
// Rand() as Rand1,
// IterNo() as TransLineID,
// RecNo() as TransID
//Autogenerate 500000
// ; //While Rand()<=0.5 or IterNo()=1;

////das ist ein Test mal sehn wi der ist                                hlkdsahfkjsdhf                        dsakfjhsdkjfh        sadf

// Comment Field Dim1 With "This is a field comment";          	  

        static void Main(string[] args)
        {         

            #region Sample1
            var location = Location.Local;
            var hub = location.Hub();
            Console.WriteLine("We are online:" + location.IsAlive().ToString());

            foreach (var app in hub.GetAppList())
            {
                Console.WriteLine(app.AppName);
            }

            var app3 = hub.OpenApp("CleanCtrlQ");
            var count = app3.Evaluate("Count(TransID)");
            Console.WriteLine(count); 
            #endregion

            #region PreSample2
            //var location = Location.FromUri("ws://127.0.0.1:4848");
            //location.AsDirectConnectionToPersonalEdition();
            #endregion

            #region Sample2
            var lb = app3.CreateGenericSessionObject(
                            new ListboxProperties()
                            {
                                Info = new NxInfo() { Type = "listbox" },
                                ListObjectDef = new ListboxListObjectDef
                                {
                                    InitialDataFetch = new NxPage[] { Pager.Default },
                                    Def = new ListboxListObjectDimensionDef()
                                    {
                                        FieldDefs = new List<string>() { "TransID" },
                                        FieldLabels = new List<string>() { "TransID" },
                                        SortCriterias = new List<SortCriteria> { new SortCriteria() { SortByState = SortDirection.Ascending } },
                                    },
                                    ShowAlternatives = true,
                                },

                            }) as IListbox;



            var dt = DateTime.Now;
            var cardinal = lb.DimensionInfo.Cardinal;
            for (int i = 0; i < cardinal; i += 10000)
            {
                var data = lb.GetListObjectData("/qListObjectDef",
                    new List<NxPage>() { 
                        new NxPage() { Top = i, Left = 0, Height = 10000, Width = 1 } 
                    });

            }
            Console.WriteLine("Time:" + (DateTime.Now - dt).TotalSeconds.ToString("0.0"));

            Console.ReadLine();
            #endregion

            #region Sample3
            dt = DateTime.Now;

            var toDoList = new List<NxPage>();
            for (int i = 0; i < cardinal; i += 10000)
            {
                toDoList.Add(new NxPage() { Top = i, Height = 10000 });
            }

            var res = toDoList.AsParallel().Select((page, result) =>
            {

                try
                {
                    string s = "";
                    var ls = lb.GetListObjectData("/qListObjectDef", new List<NxPage>() { page }).SingleOrDefault();
                    Console.WriteLine("finished page: " + page.Top.ToString());

                    return s;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }).ToList();
            Console.WriteLine("Time:" + (DateTime.Now - dt).TotalSeconds.ToString("0.0")); 
            #endregion

            Console.ReadLine();
            return;         
        }
    }
}
