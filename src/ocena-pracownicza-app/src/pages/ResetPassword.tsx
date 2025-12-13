import React, { useState } from "react";
import axios from "axios";
import { authService } from "../services/authService";

const ResetPassword: React.FC = () => {
    const [oldPassword, setOldPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [message, setMessage] = useState("");
    const [error, setError] = useState("");

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        setMessage("");
        setError("");

        try {
            await authService.changePassword(oldPassword, newPassword);

            setMessage("Has³o zosta³o zmienione.");
            setOldPassword("");
            setNewPassword("");
        } catch (err: unknown) {
            console.error(err);

            let backendMessage = "Wyst¹pi³ b³¹d.";

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
                <h1 className="text-xl font-semibold mb-4">
                    Zmieñ has³o
                </h1>

                {message && (
                    <p className="text-green-600 mb-3">{message}</p>
                )}

                {error && (
                    <p className="text-red-600 mb-3">{error}</p>
                )}

                <form onSubmit={handleSubmit}>
                    <div className="mb-4">
                        <label className="block mb-1">Stare has³o</label>
                        <input
                            type="password"
                            className="w-full border px-3 py-2 rounded"
                            value={oldPassword}
                            onChange={(e) => setOldPassword(e.target.value)}
                            required
                        />
                    </div>

                    <div className="mb-4">
                        <label className="block mb-1">Nowe has³o</label>
                        <input
                            type="password"
                            className="w-full border px-3 py-2 rounded"
                            value={newPassword}
                            onChange={(e) => setNewPassword(e.target.value)}
                            required
                        />
                    </div>

                    <button
                        type="submit"
                        className="w-full bg-black text-white py-2 rounded hover:bg-gray-800"
                    >
                        Zmieñ has³o
                    </button>
                </form>
            </div>
        </div>
    );
};

export default ResetPassword;
