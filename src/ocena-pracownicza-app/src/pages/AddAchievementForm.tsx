import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom"; // Importujemy hook do nawigacji
import axiosClient from "../services/axiosClient";

/** Typy */
export type AchievementModel = {
    name: string;
    description: string;
    date: string;
    employeeId: string;
    category: number;
    period: string;
    finalScore: string;
    achievementsSummary: string;
};

interface EmployeeOption {
    id: string;
    firstName: string;
    lastName: string;
}

type Props = {
    initialEmployeeId?: string;
    onSuccess?: () => void;
};

const KATEGORIE = [
    { id: 1, nazwa: "Sukces projektowy" },
    { id: 2, nazwa: "Rozwój techniczny" },
    { id: 3, nazwa: "Innowacja" },
    { id: 4, nazwa: "Mentoring" },
    { id: 5, nazwa: "Inne" },
];

const formatToLocal = (iso: string) => iso.slice(0, 16);

const AddAchievementForm: React.FC<Props> = ({
    initialEmployeeId = "",
    onSuccess,
}) => {
    const navigate = useNavigate(); // Inicjalizacja nawigacji
    const [employees, setEmployees] = useState<EmployeeOption[]>([]);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [loadingEmployees, setLoadingEmployees] = useState(true);
    const [message, setMessage] = useState<{
        type: "success" | "error";
        text: string;
    } | null>(null);

    const [form, setForm] = useState<AchievementModel>({
        name: "",
        description: "",
        date: new Date().toISOString(),
        employeeId: initialEmployeeId,
        category: 1,
        period: "",
        finalScore: "",
        achievementsSummary: "",
    });

    useEffect(() => {
        const fetchEmployees = async () => {
            try {
                const resp = await axiosClient.get<EmployeeOption[]>(
                    "/achievement/employee-dropdown",
                );
                setEmployees(resp.data);

                if (!initialEmployeeId && resp.data.length > 0) {
                    setForm((prev) => ({
                        ...prev,
                        employeeId: resp.data[0].id,
                    }));
                }
            } catch (err) {
                console.error("Błąd pobierania pracowników:", err);
            } finally {
                setLoadingEmployees(false);
            }
        };

        fetchEmployees();
    }, [initialEmployeeId]);

    const handleChange = (
        e: React.ChangeEvent<
            HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
        >,
    ) => {
        const { name, value } = e.target;
        setForm((prev) => ({
            ...prev,
            [name]:
                name === "category"
                    ? Number(value)
                    : name === "date"
                      ? new Date(value).toISOString()
                      : value,
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!form.employeeId) {
            setMessage({ type: "error", text: "Proszę wybrać pracownika." });
            return;
        }

        setIsSubmitting(true);
        setMessage(null);

        try {
            await axiosClient.post("/achievement", form);

            // 1. Wywołujemy opcjonalny callback sukcesu
            if (onSuccess) onSuccess();

            // 2. Przekierowujemy na listę osiągnięć
            // Zakładam, że ścieżka do listy to '/' lub '/achievements' - dostosuj wg potrzeb
            navigate("/achievements");
        } catch (error: any) {
            const errorMsg =
                error.response?.data?.message ||
                "Nie udało się zapisać osiągnięcia.";
            setMessage({ type: "error", text: errorMsg });
            setIsSubmitting(false); // Przy błędzie przywracamy przycisk
        }
    };

    return (
        <form onSubmit={handleSubmit} className="achievement-form">
            <div className="form-header">
                <button
                    type="button"
                    onClick={() => navigate(-1)}
                    className="back-btn"
                >
                    ← Wróć
                </button>
                <h2>Dodaj Osiągnięcie</h2>
            </div>

            {message && (
                <div className={`alert ${message.type}`}>{message.text}</div>
            )}

            <div className="form-group">
                <label htmlFor="employeeId">Pracownik</label>
                <select
                    id="employeeId"
                    name="employeeId"
                    value={form.employeeId}
                    onChange={handleChange}
                    required
                    disabled={loadingEmployees}
                >
                    <option value="" disabled>
                        {loadingEmployees
                            ? "Ładowanie listy..."
                            : "Wybierz pracownika"}
                    </option>
                    {employees.map((emp) => (
                        <option key={emp.id} value={emp.id}>
                            {emp.firstName} {emp.lastName}
                        </option>
                    ))}
                </select>
            </div>

            <div className="form-group">
                <label htmlFor="name">Nazwa osiągnięcia</label>
                <input
                    id="name"
                    name="name"
                    type="text"
                    value={form.name}
                    onChange={handleChange}
                    required
                />
            </div>

            <div className="form-group">
                <label htmlFor="description">Opis</label>
                <textarea
                    id="description"
                    name="description"
                    value={form.description}
                    onChange={handleChange}
                    required
                    rows={4}
                />
            </div>

            <div className="form-group">
                <label htmlFor="date">Data i godzina</label>
                <input
                    id="date"
                    name="date"
                    type="datetime-local"
                    value={formatToLocal(form.date)}
                    onChange={handleChange}
                    required
                />
            </div>

            <div className="form-group">
                <label htmlFor="category">Kategoria</label>
                <select
                    id="category"
                    name="category"
                    value={form.category}
                    onChange={handleChange}
                >
                    {KATEGORIE.map((kat) => (
                        <option key={kat.id} value={kat.id}>
                            {kat.nazwa}
                        </option>
                    ))}
                </select>
            </div>

            <div className="form-group">
                <label htmlFor="period">Okres oceny</label>
                <input
                    id="period"
                    name="period"
                    type="text"
                    value={form.period}
                    onChange={handleChange}
                    required
                />
            </div>

            <div className="form-group">
                <label htmlFor="finalScore">Wynik końcowy</label>
                <input
                    id="finalScore"
                    name="finalScore"
                    type="text"
                    value={form.finalScore}
                    onChange={handleChange}
                    required
                />
            </div>

            <div className="form-group">
                <label htmlFor="achievementsSummary">Podsumowanie osiągnięć</label>
                <textarea
                    id="achievementsSummary"
                    name="achievementsSummary"
                    value={form.achievementsSummary}
                    onChange={handleChange}
                    required
                    rows={3}
                />
            </div>

            <button
                type="submit"
                className="submit-btn"
                disabled={isSubmitting || loadingEmployees}
            >
                {isSubmitting ? "Zapisywanie..." : "Zapisz i wróć do listy"}
            </button>

            <style>{`
                .achievement-form {
                    max-width: 550px;
                    margin: 2rem auto;
                    display: grid;
                    gap: 1.25rem;
                    padding: 2.5rem;
                    background: #ffffff;
                    border-radius: 12px;
                    box-shadow: 0 10px 25px rgba(0,0,0,0.05);
                    font-family: 'Inter', sans-serif;
                }
                .form-header { display: flex; align-items: center; gap: 1rem; margin-bottom: 0.5rem; }
                .back-btn { background: none; border: none; color: #4e73df; cursor: pointer; font-weight: 600; }
                .form-group { display: flex; flex-direction: column; gap: 0.4rem; }
                label { font-size: 0.85rem; font-weight: 700; color: #4e5d78; }
                input, textarea, select { 
                    padding: 12px; border: 1px solid #dce1e8; border-radius: 8px; 
                    background-color: #f9fbff; color: #2e3b4e; font-size: 0.95rem;
                }
                input:focus { outline: none; border-color: #4e73df; box-shadow: 0 0 0 3px rgba(78,115,223,0.1); }
                .submit-btn { 
                    margin-top: 1rem; padding: 14px; cursor: pointer; background: #4e73df; 
                    color: white; border: none; border-radius: 8px; font-weight: 700; font-size: 1rem;
                    transition: all 0.2s;
                }
                .submit-btn:hover { background: #2e59d9; transform: translateY(-1px); }
                .submit-btn:disabled { background: #cbd5e0; cursor: not-allowed; transform: none; }
                .alert { padding: 12px; border-radius: 8px; text-align: center; font-size: 0.9rem; font-weight: 600; }
                .alert.success { background: #c6f6d5; color: #22543d; }
                .alert.error { background: #fed7d7; color: #822727; }
            `}</style>
        </form>
    );
};

export default AddAchievementForm;
