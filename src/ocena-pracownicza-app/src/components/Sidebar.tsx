import React, { useEffect, useEffectEvent, useState } from "react";
import { Drawer, Toolbar, List, Box } from "@mui/material";
import SidebarItem from "./SidebarItem";
import HomeIcon from "@mui/icons-material/Home";
import PeopleIcon from "@mui/icons-material/People";
import PersonIcon from "@mui/icons-material/Person";
import SettingsIcon from "@mui/icons-material/Settings";
import { authService } from "../services/authService";
import { useAuth } from "../hooks/AuthProvider";

// Zakładam, że masz hook useAuth. Jeśli nie, na razie podaję wersję z symulacją.
const Sidebar: React.FC<{ drawerWidth: number }> = ({ drawerWidth }) => {
    const { user } = useAuth();

    const matchPathProfile = () => {
        if (user?.role === "Admin") return "/admin";
        else if (user?.role === "Employee") return "/employee";
        else if (user?.role === "Manager") return "/manager";
        return "/employee";
    };

    return (
        <Drawer
            variant="permanent"
            sx={{
                width: drawerWidth,
                flexShrink: 0,
                "& .MuiDrawer-paper": {
                    width: drawerWidth,
                    boxSizing: "border-box",
                    bgcolor: "background.paper",
                    borderRight: "1px solid #D1D5DB",
                },
            }}
        >
            <Toolbar />
            <Box sx={{ overflow: "auto", p: 2 }}>
                <List>
                    <SidebarItem
                        text="Dashboard"
                        to="/"
                        IconComponent={HomeIcon}
                    />
                    <SidebarItem
                        text="Profil"
                        to={matchPathProfile()}
                        IconComponent={PersonIcon}
                    />

                    {/* Warunkowe wyświetlanie - kluczowe dla Panelu Admina */}
                    {user?.role === "Admin" && (
                        <SidebarItem
                            text="Użytkownicy"
                            to="/users"
                            IconComponent={PeopleIcon}
                        />
                    )}

                    <SidebarItem
                        text="Ustawienia"
                        to="/settings"
                        IconComponent={SettingsIcon}
                    />
                </List>
            </Box>
        </Drawer>
    );
};
export default Sidebar;
