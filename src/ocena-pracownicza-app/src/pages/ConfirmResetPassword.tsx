/* @jsxImportSource react */
import { useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import axios from "axios";
import { authService } from "../services/authService";

export default function ConfirmResetPassword() {
    const navigate = useNavigate();
    const [params] = useSearchParams();

    // Token z URL
    const tokenFromUrl = params.get("token") ?? "";

    const [password, setPassword] = useState("");
    const [password2, setPassword2] = useState("");
    const [status, setStatus] = useState<"idle" | "loading" | "success" | "error">("idle");
    const [message, setMessage] = useState<string | null>(null);

    //  prawid³owy, bezpieczny typ zdarzenia
    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        if (password !== password2) {
            setStatus("error");
            setMessage("Has³a musz¹ byæ takie same.");
            return;
        }

        if (!tokenFromUrl) {
            setStatus("error");
            setMessage("Brak tokenu resetuj¹cego.");
            return;
        }

        setStatus("loading");

        try {
            const res = await authService.confirmResetPassword({
                token: tokenFromUrl,
                newPassword: password,
            });

            setMessage(res.message);
            setStatus("success");

            setTimeout(() => navigate("/login"), 2000);
        } catch (error: unknown) {
            let backendMessage = "Wyst¹pi³ b³¹d.";

            //  Bezpieczna obs³uga AxiosError
            if (axios.isAxiosError(error)) {
                const data = error.response?.data;

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

            setMessage(backendMessage);
            setStatus("error");
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center p-4">
            <div className="w-full max-w-md bg-white shadow rounded-xl p-6">
                <h1 className="text-xl font-semibold mb-4">Ustaw nowe has³o</h1>

                <form onSubmit={handleSubmit} className="space-y-4">
                    <div>
                        <label className="block text-sm font-medium mb-1">Nowe has³o</label>
                        <input
                            type="password"
                            className="w-full border rounded px-3 py-2"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-medium mb-1">Powtórz has³o</label>
                        <input
                            type="password"
                            className="w-full border rounded px-3 py-2"
                            value={password2}
                            onChange={(e) => setPassword2(e.target.value)}
                            required
                        />
                    </div>

                    <button
                        type="submit"
                        className="w-full py-2 rounded bg-blue-600 text-white"
                    >
                        Zapisz nowe has³o
                    </button>
                </form>

                {status === "loading" && (
                    <p className="mt-3 text-gray-600">Przetwarzanie...</p>
                )}
                {status === "success" && (
                    <p className="mt-3 text-green-600">{message}</p>
                )}
                {status === "error" && (
                    <p className="mt-3 text-red-600">{message}</p>
                )}

                <div className="mt-4">
                    <button onClick={() => navigate("/login")} className="text-sm underline">
                        Powrót do logowania
                    </button>
                </div>
            </div>
        </div>
    );
}
