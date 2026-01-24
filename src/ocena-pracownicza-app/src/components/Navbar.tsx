import React, { useEffect, useState } from "react";
import {
    AppBar,
    Toolbar,
    Typography,
    Box,
    Button,
    Avatar,
} from "@mui/material";
import DashboardIcon from "@mui/icons-material/Dashboard";
import { useNavigate } from "react-router-dom";
import { authService } from "../services/authService";
import { User } from "../utils/types";
import { useAuth } from "../hooks/AuthProvider";

interface NavbarProps {
    drawerWidth: number;
    user?: User | null;
}

const Navbar: React.FC<NavbarProps> = ({ drawerWidth, user }) => {
    const navigate = useNavigate();
    const { logout } = useAuth();

    return (
        <AppBar
            position="fixed"
            elevation={0}
            sx={{
                width: "100%",
                bgcolor: "background.paper",
                borderBottom: "1px solid #D1D5DB",
                zIndex: (theme) => theme.zIndex.drawer + 1,
            }}
        >
            <Toolbar disableGutters>
                <Box
                    sx={{
                        width: drawerWidth,
                        display: "flex",
                        alignItems: "center",
                        height: "100%",
                        px: 2,
                        borderRight: "1px solid #D1D5DB",
                    }}
                >
                    <DashboardIcon
                        color="primary"
                        sx={{ mr: 1, fontSize: 24 }}
                    />
                    <Typography
                        variant="h6"
                        noWrap
                        color="text.primary"
                        fontWeight="600"
                    >
                        Ocena Pracownicza
                    </Typography>
                </Box>

                <Box
                    sx={{
                        flexGrow: 1,
                        display: "flex",
                        justifyContent: "flex-end",
                        alignItems: "center",
                        px: 2,
                    }}
                >
                    {!user ? (
                        <Button
                            variant="contained"
                            color="primary"
                            onClick={() => navigate("/login")}
                        >
                            Zaloguj się
                        </Button>
                    ) : (
                        <Button
                            variant="contained"
                            color="primary"
                            onClick={logout}
                        >
                            Wyloguj się
                        </Button>
                    )}
                </Box>
            </Toolbar>
        </AppBar>
    );
};

export default Navbar;
