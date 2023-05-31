use crate::skills::*;
#[derive(Debug, Clone, rustler::NifStruct)]
#[module = "DarkWorldsServer.Engine.Character"]
pub struct Character {
    pub class: Class,
    pub name: String,
    pub speed: u64,
    pub basic_skill: BasicSkill,
}
impl Character {
    pub fn new(class: Class, speed: u64, name: &str, basic_skill: BasicSkill) -> Self {
        Self {
            class,
            name: name.into(),
            basic_skill,
            speed,
        }
    }
    #[inline]
    pub fn attack_dmg(&self) -> u64 {
        // TODO have a trait for this
        // instead of matching enums.
        match self.basic_skill {
            BasicSkill::Slingshot => 10_u64,
            BasicSkill::Bash => 40_u64,
        }
    }
    // Cooldown in seconds
    #[inline]
    pub fn cooldown(&self) -> u64 {
        match self.basic_skill {
            BasicSkill::Slingshot => 1,
            BasicSkill::Bash => 5,
        }
    }
}
impl Default for Character {
    fn default() -> Self {
        Character::new(Class::Hunter, 5, "H4ck", BasicSkill::Slingshot)
    }
}
