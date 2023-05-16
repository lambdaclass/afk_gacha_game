use crate::skills::*;
use rustler::{Decoder, Encoder};
#[derive(Debug, Clone, rustler::NifStruct)]
#[module = "DarkWorldsServer.Engine.Character"]
pub struct Character {
    pub class: Class,
    pub speed: u64,
    pub name: String,
    pub basic_skill: BasicSkill,
}
impl Character {
    pub fn new(class: Class, speed: u64, name: &str, basic_skill: BasicSkill) -> Self {
        Self {
            class,
            speed,
            name: name.into(),
            basic_skill,
        }
    }
    #[inline]
    pub fn attack_dmg(&self) -> i64 {
        // TODO have a trait for this
        // instead of matching enums.
        match self.basic_skill {
            BasicSkill::Backstab => -10,
        }
    }
    // Cooldown in miliseconds
    #[inline]
    pub fn cooldown(&self) -> u64 {
        match self.basic_skill {
            BasicSkill::Backstab => 5_000,
        }
    }
    #[inline]
    pub fn speed(&self) -> u64 {
        self.speed
    }
}
impl Default for Character {
    fn default() -> Self {
        Self {
            class: Class::Assassin,
            speed: 3,
            name: "Uma".into(),
            basic_skill: BasicSkill::Backstab,
        }
    }
}
