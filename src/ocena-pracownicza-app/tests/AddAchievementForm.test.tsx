import { describe, it, expect, beforeEach, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import AddAchievementForm from "../src/pages/AddAchievementForm";
import axiosClient from "../src/services/axiosClient";

const navigateMock = vi.fn();

vi.mock("react-router-dom", async () => {
    const actual = await vi.importActual<typeof import("react-router-dom")>(
        "react-router-dom",
    );

    return {
        ...actual,
        useNavigate: () => navigateMock,
    };
});

vi.mock("../src/services/axiosClient", () => ({
    default: {
        get: vi.fn(),
        post: vi.fn(),
    },
}));

describe("AddAchievementForm", () => {
    beforeEach(() => {
        vi.clearAllMocks();

        vi.mocked(axiosClient.get).mockImplementation((url: string) => {
            if (url === "/employee/me") {
                return Promise.resolve({
                    data: {
                        data: {
                            id: "employee-1",
                            firstName: "Jan",
                            lastName: "Kowalski",
                        },
                    },
                });
            }

            if (url.startsWith("/evaluation-periods/by-date")) {
                return Promise.resolve({
                    data: {
                        id: "period-1",
                        name: "2026",
                    },
                });
            }

            return Promise.reject(new Error(`Unexpected GET ${url}`));
        });
    });

    it("requires completeness confirmation before final submission", async () => {
        vi.mocked(axiosClient.post).mockResolvedValue({ data: { id: "achievement-1" } });

        render(<AddAchievementForm />);

        await screen.findByDisplayValue("Jan Kowalski");

        fireEvent.change(screen.getByLabelText("Nazwa osiągnięcia"), {
            target: { value: "Projekt X" },
        });
        fireEvent.change(screen.getByLabelText("Opis"), {
            target: { value: "Kompletny opis osiągnięcia" },
        });
        fireEvent.change(screen.getByLabelText("Wynik końcowy"), {
            target: { value: "5" },
        });
        fireEvent.change(screen.getByLabelText("Podsumowanie osiągnięć"), {
            target: { value: "Podsumowanie gotowe do wysłania" },
        });

        fireEvent.click(screen.getByRole("button", { name: "Prześlij do oceny" }));

        expect(
            await screen.findByRole("heading", {
                name: "Potwierdź kompletność zgłoszenia",
            }),
        ).toBeInTheDocument();
        expect(axiosClient.post).not.toHaveBeenCalled();

        const confirmButton = screen.getByRole("button", {
            name: "Potwierdź i wyślij",
        });
        expect(confirmButton).toBeDisabled();

        fireEvent.click(
            screen.getByLabelText(
                "Potwierdzam, że zgłoszenie jest kompletne i gotowe do wysłania do przełożonego.",
            ),
        );

        expect(confirmButton).toBeEnabled();

        fireEvent.click(confirmButton);

        await waitFor(() =>
            expect(axiosClient.post).toHaveBeenCalledWith(
                "/achievement",
                expect.objectContaining({
                    name: "Projekt X",
                    isDraft: false,
                    employeeId: "employee-1",
                    evaluationPeriodId: "period-1",
                }),
            ),
        );
        expect(navigateMock).toHaveBeenCalledWith("/achievements");
    });

    it("keeps draft submission without the confirmation step", async () => {
        vi.mocked(axiosClient.post).mockResolvedValue({ data: { id: "achievement-2" } });

        render(<AddAchievementForm />);

        await screen.findByDisplayValue("Jan Kowalski");

        fireEvent.click(
            screen.getByRole("button", { name: "Zapisz jako wersję roboczą" }),
        );

        await waitFor(() =>
            expect(axiosClient.post).toHaveBeenCalledWith(
                "/achievement",
                expect.objectContaining({
                    isDraft: true,
                    employeeId: "employee-1",
                    evaluationPeriodId: "period-1",
                }),
            ),
        );
        expect(
            screen.queryByRole("heading", {
                name: "Potwierdź kompletność zgłoszenia",
            }),
        ).not.toBeInTheDocument();
    });
});
