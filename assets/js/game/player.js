export class Player{
    constructor(number, game_id) {
        this.number = number
        this.socket = new WebSocket(this.getplayConnection(game_id))
    }

    move(direction) {
        this.socket.send(this.createMoveMessage(direction))
    }

    createMoveMessage(direction) {
        let msg = {
            player: this.number,
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
