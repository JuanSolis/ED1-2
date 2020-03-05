using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ecommerceED1_2.Models;
using ecommerceED1_2.Utils;


namespace ecommerceED1_2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {

            var fileName = string.Empty;
            var path = string.Empty;
            if (file.ContentLength > 0)
            {
                fileName = Path.GetFileName(file.FileName);
                path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                file.SaveAs(path);
            }

            reader myReader = new reader(Server.MapPath("~/App_Data/uploads/" + fileName));
            ArbolBB arbolBinario = new ArbolBB();
            Nodo<String, int> primerNodo = new Nodo<string, int>();
            primerNodo.nomFarmaco = Storage.Instance.listaFarmacos[0].nombreFarmaco;
            primerNodo.lineaArchivo = (Storage.Instance.listaFarmacos[0].id + 1);

            Nodo<string, int> raiz = arbolBinario.InsertarNodo(primerNodo.lineaArchivo, primerNodo.nomFarmaco, null);

            foreach (var farmacos in Storage.Instance.listaFarmacos)
            {
                arbolBinario.InsertarNodo(farmacos.id, farmacos.nombreFarmaco, raiz);

            }
            //Prueba para saber que el arbol esta bien
            //arbolBinario.Tranversal(raiz);

            return RedirectToAction("Farmacos");
        }

        public ActionResult Farmacos()
        {

            return View(Storage.Instance.listaFarmacos);
        }

        [HttpPost]
        public ActionResult Farmacos(string id, FormCollection collection)
        {

            int idFarmaco = Convert.ToInt32(id);
            int cantidad = int.Parse(collection[("CantidadFarmaco_" + id)]);
            Farmacos farmacoAgregado = new Farmacos
            {
                id = Storage.Instance.listaFarmacos[(idFarmaco - 1)].id,
                nombreFarmaco = Storage.Instance.listaFarmacos[(idFarmaco - 1)].nombreFarmaco,
                descripcionFarmaco = Storage.Instance.listaFarmacos[(idFarmaco - 1)].descripcionFarmaco,
                casaProductora = Storage.Instance.listaFarmacos[(idFarmaco - 1)].casaProductora,
                precio = Storage.Instance.listaFarmacos[(idFarmaco - 1)].precio,
                existencia = Storage.Instance.listaFarmacos[(idFarmaco - 1)].existencia,
            };

            FarmacosPedidos farmacoPedido = new FarmacosPedidos
            {
                FarmacoPedido = farmacoAgregado,
                cantidadSolicitada = cantidad
            };

            Storage.Instance.totalACancelar += farmacoPedido.calcularTotal();
            Storage.Instance.pedidosFarmacos.Add(farmacoPedido);
            return View(Storage.Instance.listaFarmacos);
        }

    }
}