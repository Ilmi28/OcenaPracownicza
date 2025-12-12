import React, { useState } from "react";
import axios from "axios";
import {authService} from "../services/authService";

const ResetPassword: React.FC = () => {
    const [email, setEmail] = useState("");
    const [message, setMessage] = useState("");
    const [error, setError] = useState("");

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        setMessage("");
        setError("");

        try {
            await authService.resetPassword(email);

            setMessage("Jeœli email istnieje, wys³ano link resetuj¹cy.");
            setEmail("");
        } catch (err: unknown) {
            console.error(err);

            // Domyœlny komunikat gdy nie mamy szczegó³ów z backendu
            let backendMessage = "Wyst¹pi³ b³¹d.";

            // Jeœli to b³¹d axios, spróbuj wyci¹gn¹æ message z response.data
            if (axios.isAxiosError(err) && err.response?.data) {
                const data = err.response.data;

                if (typeof data === "string" && data.trim()) {
                    backendMessage = data.trim();
                } else if (
                    typeof data === "object" &&
                    data !== null &&
                    "message" in data &&
                    typeof (data as { message?: unknown }).message === "string"
                ) {
                    backendMessage = (data as { message: string }).message;
                }
            }

            setError(backendMessage);
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center p-4">
            <div className="w-full max-w-md bg-white shadow rounded-xl p-6">

                <h1 className="text-xl font-semibold mb-4">Reset has³a</h1>

                {message && (
                    <p className="text-green-600 mb-3">{message}</p>
                )}

                {error && (
                    <p className="text-red-600 mb-3">{error}</p>
                )}

                <form onSubmit={handleSubmit}>
                    <div className="mb-4">
                        <label className="block mb-1">Email</label>
                        <input
                            type="email"
                            className="w-full border px-3 py-2 rounded"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                        />
                    </div>

                    <button
                        type="submit"
                        className="w-full bg-black text-white py-2 rounded hover:bg-gray-800"
                    >
                        Resetuj has³o
                    </button>
                </form>

            </div>
        </div>
    );
};

export default ResetPassword;
