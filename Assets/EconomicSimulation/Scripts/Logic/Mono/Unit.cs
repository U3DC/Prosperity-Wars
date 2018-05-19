﻿using Nashet.EconomicSimulation;
using Nashet.UnityUIUtils;
using Nashet.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Nashet.EconomicSimulation
{
    public class Unit : MonoBehaviour, IHasCountry, IHasProvince
    {
        //[SerializeField]
        //private int ID;
        //[SerializeField]
        //private Province province;

        [SerializeField]
        private GameObject selectionPart;

        [SerializeField]
        private float unitPanelYOffset = -2f;

        //[SerializeField]
        private LineRenderer lineRenderer;

        private GameObject unitPanelObject;
        private UnitPanel unitPanel;
        Animator m_Animator;


        private readonly static List<Unit> allUnits = new List<Unit>();

        public Army NextArmy
        {
            get
            {
                return Game.Player.getAllArmies().Where(x => x.Province == Province).Random();
            }
        }

        public Province Province { get; private set; }

        public Country Country { get; private set; }

        void Awake()
        {
            m_Animator = GetComponent<Animator>();
            allUnits.Add(this);
            lineRenderer = selectionPart.GetComponent<LineRenderer>();
            selectionPart.SetActive(false);


            gameObject.SetActive(true);
            unitPanelObject = Instantiate(LinksManager.Get.UnitPanelPrefab, LinksManager.Get.WorldSpaceCanvas);
            unitPanel = unitPanelObject.GetComponent<UnitPanel>();
            //unitPanel.Set(currentProvince.Country.Flag);
            //SetUnitPanel();
        }
        public static Unit Create(Army army)
        {
            var unitObject = GameObject.Instantiate(LinksManager.Get.UnitPrefab, World.Get.transform);
            unitObject.name = (World.GetAllProvinces().Count() + UnityEngine.Random.Range(0, 2000)).ToString();

            unitObject.transform.position = army.Position;

            var unit = unitObject.GetComponent<Unit>();
            unit.Province = army.Province;
            unit.Country = army.getOwner().Country;
            unit.SetUnitPanel(army);

            return unit;
        }

        internal void Select()
        {
            selectionPart.SetActive(true);
            ArmiesSelectionWindow.Get.Show();
        }

        internal void DeSelect()
        {
            selectionPart.SetActive(false);
            ArmiesSelectionWindow.Get.Refresh();
        }

        private void SetUnitPanel(Army army)
        {
            var panelPosition = gameObject.transform.position;
            panelPosition.y += unitPanelYOffset;
            panelPosition.z = -1f;
            unitPanelObject.transform.position = panelPosition;
            UpdateShield();

            unitPanel.SetFlag(army.getOwner().Country.Flag);
        }
        public static IEnumerable<Unit> AllUnits()
        {
            foreach (var item in allUnits)
            {
                yield return item;
            }
        }
        public void Simulate()
        { }

        internal static Unit FindByID(int meshNumber)
        {
            return allUnits.Find(x => Int32.Parse(x.name) == meshNumber);
        }




        public override string ToString()
        {
            return "Army";
        }

        internal void UpdateShield()
        {
            int count = 0;
            //var sb = new StringBuilder();
            int size = 0;
            foreach (var item in Country.getAllArmies().Where(x => x.Province == this.Province))
            {
                size += item.getSize();
                count++;
            }
            if (count > 1)
                unitPanel.SetText(size + ":" + count);
            else
                unitPanel.SetText(size.ToString());
        }
    }
}