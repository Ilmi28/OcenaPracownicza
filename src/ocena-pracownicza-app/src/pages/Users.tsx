//src/pages/Users.tsx
import React, { useEffect, useState } from "react";
import {
    Box,
    Typography,
    Button,
    Paper,
    TextField,
    InputAdornment,
    Select,
    MenuItem,
    FormControl,
    Tabs,
    Tab,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Chip,
    CircularProgress,
} from "@mui/material";

import SearchIcon from "@mui/icons-material/Search";
import AddIcon from "@mui/icons-material/Add";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";

import { IUser } from "../models/User";
import { userService } from "../services/userService";

const Users = () => {
    const [activeTab, setActiveTab] = useState(0);
    const [search, setSearch] = useState("");
    const [roleFilter, setRoleFilter] = useState("all");
    const [statusFilter, setStatusFilter] = useState("all");

    const [users, setUsers] = useState<IUser[]>([]);
    const [loading, setLoading] = useState<boolean>(true);

    const handleTabChange = (_event: React.SyntheticEvent, newValue: number) => {
        setActiveTab(newValue);
    };

    useEffect(() => {
        const load = async () => {
            try {
                const data = await userService.getAll();
                setUsers(data);
            } catch (err) {
                console.error("Failed to load users", err);
            } finally {
                setLoading(false);
            }
        };

        load();
    }, []);

    const filteredUsers = users
        .filter((u) =>
            `${u.firstName} ${u.lastName} ${u.email}`
                .toLowerCase()
                .includes(search.toLowerCase())
        )
        .filter((u) =>
            roleFilter === "all"
                ? true
                : (u.roles || []).includes(roleFilter)
        )
        .filter((u) =>
            statusFilter === "all"
                ? true
                : statusFilter === "active"
                    ? u.isActive === true
                    : u.isActive === false
        );

    return (
        <Box sx={{ flexGrow: 1 }}>
            {/* HEADER */}
            <Box
                sx={{
                    display: "flex",
                    justifyContent: "space-between",
                    alignItems: "center",
                    mb: 3,
                }}
            >
                <Box>
                    <Typography variant="h5" component="h1" fontWeight="600">
                        Zarządzanie Użytkownikami
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                        Przeglądaj i zarządzaj wszystkimi użytkownikami systemu
                    </Typography>
                </Box>
                <Button variant="contained" startIcon={<AddIcon />} sx={{ textTransform: "none" }}>
                    Dodaj użytkownika
                </Button>
            </Box>

            {/* FILTER SECTION */}
            <Paper sx={{ p: 3, mb: 3 }}>
                <Typography variant="subtitle1" fontWeight="600" sx={{ mb: 2 }}>
                    Filtry
                </Typography>

                <Box sx={{ display: "flex", gap: 2, alignItems: "center" }}>
                    <TextField
                        fullWidth
                        size="small"
                        placeholder="Szukaj użytkownika..."
                        value={search}
                        onChange={(e) => setSearch(e.target.value)}
                        InputProps={{
                            startAdornment: (
                                <InputAdornment position="start">
                                    <SearchIcon color="action" />
                                </InputAdornment>
                            ),
                        }}
                        sx={{ flex: 1 }}
                    />

                    {/* Filter: Role */}
                    <FormControl sx={{ minWidth: 180 }} size="small">
                        <Select
                            value={roleFilter}
                            onChange={(e) => setRoleFilter(e.target.value)}
                            displayEmpty
                        >
                            <MenuItem value="all">Wszystkie role</MenuItem>
                            <MenuItem value="User">Pracownik</MenuItem>
                            <MenuItem value="Manager">Przełożony</MenuItem>
                            <MenuItem value="Admin">Administrator</MenuItem>
                        </Select>
                    </FormControl>

                    {/* Filter: Active/Inactive */}
                    <FormControl sx={{ minWidth: 180 }} size="small">
                        <Select
                            value={statusFilter}
                            onChange={(e) => setStatusFilter(e.target.value)}
                            displayEmpty
                        >
                            <MenuItem value="all">Wszystkie statusy</MenuItem>
                            <MenuItem value="active">Aktywny</MenuItem>
                            <MenuItem value="inactive">Nieaktywny</MenuItem>
                        </Select>
                    </FormControl>
                </Box>
            </Paper>

            {/* TABS (Role Groups) */}
            <Paper sx={{ mb: 3 }}>
                <Tabs
                    value={activeTab}
                    onChange={handleTabChange}
                    indicatorColor="primary"
                    textColor="primary"
                    sx={{ borderBottom: 1, borderColor: "divider" }}
                >
                    <Tab label="Pracownicy" sx={{ textTransform: "none" }} />
                    <Tab label="Przełożeni" sx={{ textTransform: "none" }} />
                    <Tab label="Administratorzy" sx={{ textTransform: "none" }} />
                </Tabs>
            </Paper>

            {/* USERS TABLE */}
            <Paper sx={{ p: 3, mb: 3 }}>
                <Box sx={{ mb: 2 }}>
                    <Typography variant="subtitle1" fontWeight="600">
                        Lista użytkowników
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                        Wszyscy użytkownicy z systemu
                    </Typography>
                </Box>

                {loading ? (
                    <Box sx={{ textAlign: "center", p: 4 }}>
                        <CircularProgress />
                    </Box>
                ) : (
                    <TableContainer>
                        <Table size="small">
                            <TableHead>
                                <TableRow>
                                    {["Imię", "Nazwisko", "E-mail", "Role", "Status", "Akcje"].map(
                                        (header) => (
                                            <TableCell
                                                key={header}
                                                sx={{ color: "text.secondary", fontWeight: "600" }}
                                            >
                                                {header}
                                            </TableCell>
                                        )
                                    )}
                                </TableRow>
                            </TableHead>

                            <TableBody>
                                {filteredUsers.map((user) => (
                                    <TableRow
                                        key={user.id}
                                        sx={{ "&:hover": { bgcolor: "action.hover" } }}
                                    >
                                        <TableCell>{user.firstName}</TableCell>
                                        <TableCell>{user.lastName}</TableCell>
                                        <TableCell>{user.email}</TableCell>

                                        {/* ROLES */}
                                        <TableCell>
                                            {(user.roles || []).map((role) => (
                                                <Chip
                                                    key={role}
                                                    label={role}
                                                    size="small"
                                                    sx={{
                                                        mr: 0.5,
                                                        bgcolor: "#E3F2FD",
                                                        color: "primary.main",
                                                        fontWeight: "600",
                                                    }}
                                                />
                                            ))}
                                        </TableCell>

                                        {/* ACTIVE / INACTIVE */}
                                        <TableCell>
                                            <Chip
                                                label={user.isActive ? "Aktywny" : "Nieaktywny"}
                                                size="small"
                                                sx={{
                                                    bgcolor: user.isActive ? "#E6F4EA" : "#FDECEA",
                                                    color: user.isActive ? "success.main" : "error.main",
                                                    fontWeight: "600",
                                                }}
                                            />
                                        </TableCell>

                                        {/* ACTIONS */}
                                        <TableCell>
                                            <Box sx={{ display: "flex", gap: 0.5 }}>
                                                <EditIcon
                                                    color="action"
                                                    sx={{ fontSize: 18, cursor: "pointer" }}
                                                />
                                                <DeleteIcon
                                                    color="error"
                                                    sx={{ fontSize: 18, cursor: "pointer" }}
                                                />
                                            </Box>
                                        </TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    </TableContainer>
                )}
            </Paper>
        </Box>
    );
};

export default Users;
