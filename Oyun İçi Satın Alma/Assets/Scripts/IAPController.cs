using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;//Kutuphanemizi ekledik(satin alma islemleri icin)
using System;//Kutuphanemizi ekledik
using UnityEngine.UI;//Kutuphanemizi ekledik (text ekledigimiz icin)
public class IAPController : MonoBehaviour ,IStoreListener
{
    public string[] product;//buraya satin alinabilir ogelerimizi ekleyecegiz
    public Text coinText;
    IStoreController controller;
    public bool delete = true;
    private void Start()
    {
        if (delete)
            PlayerPrefs.DeleteAll();
        IAPStart();
        if (PlayerPrefs.GetInt("RemoveAds") == 1)//removeads i satin almis miyiz onu kontrol ediyoruz
        {
            GameObject.Find("Reklam").SetActive(false);
            GameObject.Find("ReklamKaldirButon").GetComponent<Button>().interactable=false;
        }
    }
    private void IAPStart()
    {
        var module = StandardPurchasingModule.Instance();
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);
        foreach(string item in product)
        {
            builder.AddProduct(item,ProductType.Consumable);//urunleri buildera gönderip product ekliyoruz urun tipini tuketilebilir verdik
        }
        UnityPurchasing.Initialize(this, builder);
    }
    //Burdaki fonksiyonlar interfaceden gelen fonksiyonlar 
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        this.controller = controller;//8. satirda tanimladigimiz controlleri ýnterfacedeki controllere eþitliyoruz
    }
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("Hata" + error.ToString());//islemde hata alirsak burasi calisacak ve consola hatayi yazacak
    }
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log("Satin Alma Esnasinda Hata Olustu" + failureReason.ToString());//satin alma sirasinda hata olusursa burasi calisacak
    }
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)//bu fonksiyonda gelen butona gore satin alim yapiyoruz
    {
        if (string.Equals(e.purchasedProduct.definition.id, product[0],StringComparison.Ordinal))// dzinin 0. elemani gelirse (100 altina denk gelen buton)
        {
            AddCoin(100);
            return PurchaseProcessingResult.Complete;
        }
        else if (string.Equals(e.purchasedProduct.definition.id, product[1], StringComparison.Ordinal))// dzinin 1. elemani gelirse (200 altina denk gelen buton)
        {
            AddCoin(200);
            return PurchaseProcessingResult.Complete;
        }
        else if (string.Equals(e.purchasedProduct.definition.id, product[2], StringComparison.Ordinal))// dzinin 2. elemani gelirse (reklam kaldirmaya denk gelen buton)
        {
            RemoveAds();
            return PurchaseProcessingResult.Complete;
        }
        else
        {
            return PurchaseProcessingResult.Pending;
        }
        
    }
    //

    private void AddCoin(int coin)//bu fonksiyonda gelen coin kadar coin alindi bilgisi verdik
    {
        coinText.text = coin.ToString() + " satýn alýndý";
    }
    private void RemoveAds()//bu fonksiyonda reklami kaldirma bir defa satin alindiysa bir daha satin alinmamasini kontrol ettik
    {
        PlayerPrefs.SetInt("RemoveAds", 1);
        GameObject.Find("Reklam").SetActive(false);
        GameObject.Find("ReklamKaldirButon").GetComponent<Button>().interactable = false;
    }
    public void IAPButton(string id)//burda idsi gelen buton ile controlleri bagladik
    {
        Product proc = controller.products.WithID(id);
        if(proc !=null && proc.availableToPurchase)
        {
            Debug.Log("Satýn Alýnýyor");
            controller.InitiatePurchase(proc);
        }
        else
        {
            Debug.Log("Satin Alinamadi");
        }
    }
    //controller module ve listener IStoreListener(Ýnterface) ile iletiþim için yazýlýdý.
    //Burada IStoreListener e sað týk yapýp tanýma git demeniz gerek oradaki kodlara ihtiyacýmýz olacak.
}
