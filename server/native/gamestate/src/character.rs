use crate::skills::*;
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
    pub fn attack_dmg(&self) -> u64 {
        // TODO have a trait for this
        // instead of matching enums.
        match self.basic_skill {
            BasicSkill::Slingshot => 10_u64,
        }
    }
    // Cooldown in miliseconds
    #[inline]
    pub fn cooldown(&self) -> u64 {
        match self.basic_skill {
            BasicSkill::Slingshot => 5_000,
        }
    }
}
impl Default for Character {
    fn default() -> Self {
        Character::new(Class::Hunter, 3, "H4ck", BasicSkill::Slingshot)
    }
}
