use rustler::NifStruct;
use rustler::NifUnitEnum;

#[derive(Debug, Clone, NifStruct)]
#[module = "DarkWorldsServer.Engine.Projectile"]
pub struct Projectile {
    pub id: u64,
    pub position: Position,
    pub direction: JoystickValues,
    pub speed: u32,
    pub range: u32,
    pub player_id: u64,
    pub damage: u32,
    pub remaining_ticks: u32,
    pub projectile_type: ProjectileType,
}

#[derive(Debug, Clone, NifUnitEnum)]
pub enum ProjectileType {
    BULLET,
}

#[derive(Debug, Clone, NifStruct, PartialEq)]
#[module = "DarkWorldsServer.Engine.Position"]
pub struct Position {
    pub x: usize,
    pub y: usize,
}

impl Position {
    pub fn new(x: usize, y: usize) -> Self {
        Self { x, y }
    }
}

#[derive(Debug, Clone, NifStruct, PartialEq)]
#[module = "DarkWorldsServer.Engine.JoystickValues"]
pub struct JoystickValues {
    pub x: i64,
    pub y: i64,
}

impl JoystickValues {
    pub fn new(x: i64, y: i64) -> Self {
        Self { x, y }
    }
}

impl Projectile {
    pub fn new(id: u64) -> Self {
        Self {
            id,
            position: Position::new(50, 50),
            direction: JoystickValues::new(1, 0),
            speed: 1,
            range: 1,
            player_id: 1,
            damage: 10,
            remaining_ticks: 10,
            projectile_type: ProjectileType::BULLET,
        }
    }
}
