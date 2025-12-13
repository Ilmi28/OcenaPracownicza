import React, { useState } from "react";
import {
    Box,
    Paper,
    Typography,
    TextField,
    Button,
    Alert,
    Link,
    Divider,
    List,
    ListItem,
    ListItemText,
} from "@mui/material";
import { useNavigate } from "react-router-dom";
import { authService } from "../services/authService";

const Login: React.FC = () => {
    const [login, setLogin] = useState("");
    const [password, setPassword] = useState("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const navigate = useNavigate();

    const handleLogin = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        setLoading(true);

        try {
            const response = await authService.login({
                userNameEmail: login,
                password: password,
            });
            console.log("LOGIN SUCCESS:", response);
            navigate("/");
        } catch (err) {
            console.error("LOGIN FAILED:", err);
            setError("Nieprawidłowy login lub hasło.");
        }

        setLoading(false);
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
                    Ocena Pracownicza
                </Typography>
                <Typography variant="subtitle1" gutterBottom textAlign="center">
                    System zarządzania ocenami okresowymi
                </Typography>

                <Divider sx={{ my: 3 }} />

                <Typography variant="h6" gutterBottom fontWeight={600}>
                    Zaloguj się
                </Typography>
                <Typography variant="body2" gutterBottom>
                    Wprowadź swoje dane, aby uzyskać dostęp do systemu
                </Typography>

                {error && (
                    <Alert severity="error" sx={{ mb: 2 }}>
                        {error}
                    </Alert>
                )}

                <Box
                    component="form"
                    onSubmit={handleLogin}
                    sx={{ display: "grid", gap: 2 }}
                >
                    <TextField
                        label="Adres e-mail lub nazwa użytkownika"
                        variant="outlined"
                        value={login}
                        onChange={(e) => setLogin(e.target.value)}
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

                    <Link href="#" underline="hover" sx={{ fontSize: 14 }}>
                        Zapomniałem hasła
                    </Link>

                    <Button
                        type="submit"
                        variant="contained"
                        disabled={loading}
                        sx={{ mt: 1 }}
                    >
                        {loading ? "Logowanie..." : "Zaloguj się"}
                    </Button>
                </Box>

                <Divider sx={{ my: 4 }} />

                <Typography variant="body2" fontWeight={500} gutterBottom>
                    Demo – użyj przykładowych loginów:
                </Typography>
                <List dense>
                    <ListItem>
                        <ListItemText primary="admin@firma.pl – Panel administratora" />
                    </ListItem>
                    <ListItem>
                        <ListItemText primary="manager@firma.pl – Panel przełożonego" />
                    </ListItem>
                    <ListItem>
                        <ListItemText primary="pracownik@firma.pl – Panel pracownika" />
                    </ListItem>
                </List>
            </Paper>
        </Box>
    );
};

export default Login;
