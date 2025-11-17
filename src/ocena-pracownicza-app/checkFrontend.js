const { spawn } = require('child_process');

const child = spawn('npm', ['start'], { shell: true });

let hasCriticalError = false;

child.stdout.on('data', (data) => {
    const message = data.toString();
    process.stdout.write(message);

    // SprawdŸ frazy typowe dla krytycznych b³êdów
    if (message.includes('ERROR') || message.includes('Failed')) {
        hasCriticalError = true;
    }
});

child.stderr.on('data', (data) => {
    const message = data.toString();
    process.stderr.write(message);
    hasCriticalError = true;
});

child.on('close', (code) => {
    if (hasCriticalError || code !== 0) {
        console.error('Frontend failed to start correctly');
        process.exit(1);
    } else {
        console.log('rontend started without critical errors');
        process.exit(0);
    }
});
