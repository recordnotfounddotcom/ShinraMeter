﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DamageMeter.Skills.Skill;
using Data;

namespace DamageMeter.UI
{
    /// <summary>
    ///     Logique d'interaction pour Skills.xaml
    /// </summary>
    public partial class Skills
    {
        private readonly PlayerStats _parent;
        private readonly SkillsDetail _skillDps;
        private readonly SkillsDetail _skillHeal;
        private readonly SkillsDetail _skillMana;


        public Skills(Dictionary<long, Dictionary<DamageMeter.Skills.Skill.Skill, SkillStats>> timedSkills,
            Dictionary<long, Dictionary<DamageMeter.Skills.Skill.Skill, SkillStats>> timedAllSkills, PlayerStats parent)
        {
            InitializeComponent();

            var skills = NoTimedSkills(timedSkills);
            var allSkills = NoTimedSkills(timedAllSkills);


            _skillDps = new SkillsDetail(skills, SkillsDetail.Type.Dps);
            _skillHeal = new SkillsDetail(allSkills, SkillsDetail.Type.Heal);
            _skillMana = new SkillsDetail(allSkills, SkillsDetail.Type.Mana);
            HealPanel.Content = _skillHeal;
            DpsPanel.Content = _skillDps;
            ManaPanel.Content = _skillMana;
            TabControl.SelectionChanged += TabControlOnSelectionChanged;
            _parent = parent;
            BackgroundColor.Opacity = BasicTeraData.Instance.WindowData.SkillWindowOpacity;
        }

        private Dictionary<DamageMeter.Skills.Skill.Skill, SkillStats> NoTimedSkills(
            Dictionary<long, Dictionary<DamageMeter.Skills.Skill.Skill, SkillStats>> dictionary)
        {
            var result = new Dictionary<DamageMeter.Skills.Skill.Skill, SkillStats>();
            foreach (var timedStats in dictionary)
            {
                foreach (var stats in timedStats.Value)
                {
                    if (result.ContainsKey(stats.Key))
                    {
                        result[stats.Key] += stats.Value;
                        continue;
                    }
                    result.Add(stats.Key, stats.Value);
                }
            }
            return result;
        }

        private void TabControlOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            var tabitem = (TabItem) ((TabControl) selectionChangedEventArgs.Source).SelectedItem;
            var skills = (SkillsDetail) tabitem.Content;

            MaxWidth = skills.ContentWidth + 50;
            MinWidth = skills.ContentWidth - 300;
            Width = skills.ContentWidth - 300;
        }


        public void Update(Dictionary<long, Dictionary<DamageMeter.Skills.Skill.Skill, SkillStats>> timedSkills,
            Dictionary<long, Dictionary<DamageMeter.Skills.Skill.Skill, SkillStats>> timedAllSkills)
        {
            var skills = NoTimedSkills(timedSkills);
            var allSkills = NoTimedSkills(timedAllSkills);

            _skillDps.Update(skills);
            _skillHeal.Update(new Dictionary<DamageMeter.Skills.Skill.Skill, SkillStats>(allSkills));
            _skillMana.Update(new Dictionary<DamageMeter.Skills.Skill.Skill, SkillStats>(allSkills));
            HealPanel.Content = _skillHeal;
            DpsPanel.Content = _skillDps;
            ManaPanel.Content = _skillMana;
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            _parent.CloseSkills();
        }

        private void Skills_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch
            {
                Console.WriteLine(@"Exception move");
            }
        }
    }
}