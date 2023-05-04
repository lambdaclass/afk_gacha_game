export { time_now }

function time_now() {
    const date = new Date();
    let time = date.getTime();
    date.setTime(time);

    return date;
}
