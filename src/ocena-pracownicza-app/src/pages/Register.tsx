import React, { useState } from "react";
import {
    Box,
    Paper,
    Typography,
    TextField,
    Button,
    Alert,
    Divider,
} from "@mui/material";
import { useNavigate } from "react-router-dom";
import { RegisterRequest } from "../utils/types";
import axiosClient from "../services/axiosClient";

const Register: React.FC = () => {
    const [username, setUsername] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const navigate = useNavigate();

    const handleRegister = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        setLoading(true);

        const payload: RegisterRequest = {
            userName: username,
            email: email,
            password: password,
        };

        try {
            await axiosClient.post("/auth/register", payload);
            navigate("/login");
        } catch (err: any) {
            if (err.response?.data) {
                setError(err.response.data);
            } else {
                setError("Coś poszło nie tak podczas rejestracji");
            }
        } finally {
            setLoading(false);
        }
    };

    return (
        <Box sx={{ display: "flex", justifyContent: "center", mt: 10 }}>
            <Paper elevation={3} sx={{ p: 5, width: 460 }}>
                <Typography
                    variant="h4"
                    fontWeight={700}
                    gutterBottom
                    textAlign="center"
                >
                    Rejestracja
                </Typography>

                {error && (
                    <Alert severity="error" sx={{ mb: 2 }}>
                        {error}
                    </Alert>
                )}

                <Box
                    component="form"
                    onSubmit={handleRegister}
                    sx={{ display: "grid", gap: 2 }}
                >
                    <TextField
                        label="Nazwa użytkownika"
                        variant="outlined"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        required
                    />

                    <TextField
                        label="Adres e-mail"
                        type="email"
                        variant="outlined"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />

                    <TextField
                        label="Hasło"
                        type="password"
                        variant="outlined"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />

                    <Typography
                        variant="caption"
                        color="text.secondary"
                        component="div"
                    >
                        Hasło musi spełniać następujące wymagania:
                        <ul style={{ margin: "4px 0 0 16px", padding: 0 }}>
                            <li>Duża litera</li>
                            <li>Mała litera</li>
                            <li>Cyfra</li>
                            <li>Znak specjalny (np. !@#$%)</li>
                            <li>Min. 6 znaków</li>
                        </ul>
                    </Typography>

                    <Button
                        type="submit"
                        variant="contained"
                        disabled={loading}
                        sx={{ mt: 1 }}
                    >
                        {loading ? "Rejestracja..." : "Zarejestruj się"}
                    </Button>
                </Box>

                <Divider sx={{ my: 3 }} />

                <Typography variant="body2" textAlign="center">
                    Masz już konto? <a href="/login">Zaloguj się</a>
                </Typography>
            </Paper>
        </Box>
    );
};

export default Register;
