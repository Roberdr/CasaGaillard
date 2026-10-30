(() => {
    const baseUrl = "/Repartos/Planificador";
    const values = new Map();
    let requestToken = "";

    const ready = (async () => {
        const [tokenResponse, stateResponse] = await Promise.all([
            fetch(`${baseUrl}/Token`, { credentials: "same-origin" }),
            fetch(`${baseUrl}/Estado`, { credentials: "same-origin" })
        ]);
        if (!tokenResponse.ok || !stateResponse.ok) {
            throw new Error("No se pudo conectar con el almacenamiento del planificador.");
        }
        requestToken = (await tokenResponse.json()).requestToken;
        const state = await stateResponse.json();
        Object.entries(state).forEach(([key, value]) => values.set(key, value));
    })();

    window.planificadorServerStorage = true;
    window.planificadorRemoteStorage = {
        async get(key) {
            await ready;
            const value = values.get(key);
            return value == null ? null : { value };
        },
        async set(key, value) {
            await ready;
            const response = await fetch(`${baseUrl}/Estado`, {
                method: "PUT",
                credentials: "same-origin",
                headers: {
                    "Content-Type": "application/json",
                    "RequestVerificationToken": requestToken
                },
                body: JSON.stringify({ clave: key, json: value })
            });
            if (!response.ok) throw new Error("No se pudo guardar el estado del planificador.");
            values.set(key, value);
        }
    };
})();
