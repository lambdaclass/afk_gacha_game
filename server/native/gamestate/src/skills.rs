use rustler::NifTaggedEnum;
// TODO: Add misssing classes
#[derive(NifTaggedEnum, Debug, Clone)]
pub enum Class {
    Assassin,
}
// TODO: Add misssing skills
#[derive(NifTaggedEnum, Debug, Clone)]
pub enum BasicSkill {
    Slingshot,
}
// TODO have a trait for this
// instead of matching enums.
// Something like:
// impl Attack for BasicSkill
