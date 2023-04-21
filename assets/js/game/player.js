export class Player{
    constructor(game_id) {
        this.socket = new WebSocket(this.getplayConnection(game_id));
        this.aimingDirection = "right"; // Set up a default direction
    }

    move(direction) {
        this.aimingDirection = direction;
        this.socket.send(this.createMoveMessage(direction))
    }

    attack() {
        this.socket.send(this.createAttackMessage())
    }

    createAttackMessage(direction){
        let msg = {
            action: "attack",
            value: this.aimingDirection
        }

        return JSON.stringify(msg)
    }

    createMoveMessage(direction) {
        let msg = {
            action: "move",
            value: direction
        }

        return JSON.stringify(msg)
    }

    getplayConnection(game_id) {
        let protocol = window.location.protocol === 'https:' ? 'wss:' : 'ws:'
        let host = window.location.host
        let path = '/play'

        return `${protocol}${host}${path}/${game_id}`
    }
}
