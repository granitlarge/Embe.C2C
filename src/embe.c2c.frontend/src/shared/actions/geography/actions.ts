"use server";

import { Read, Mutate } from "../../apis/api";
import { ApiResponse, FailureReason } from "../../apis/type";
import { AdminArea } from "./types";

export async function getAdminAreaById(id: string): Promise<ApiResponse<AdminArea>> {
    const response = await Read<AdminArea>(`${process.env.API_URL}/api/geography/${id}`, {
        method: "GET"
    }, false);
    return response;
}

export async function getCountryAdminAreas(): Promise<ApiResponse<AdminArea[]>> {

    const response = await Read<AdminArea[]>(`${process.env.API_URL}/api/geography/country`, {
        method: "GET"
    }, false);

    return response;

}

export async function searchAdminAreas(
    parentId: string | undefined,
    longitude: number | undefined,
    latitude: number | undefined,
    page: number | undefined,
    pageSize: number | undefined
): Promise<ApiResponse<AdminArea[]>> {
    const queryParams = [
        parentId ? `parentId=${parentId}` : "",
        longitude !== undefined ? `longitude=${longitude}` : "",
        latitude !== undefined ? `latitude=${latitude}` : "",
        page !== undefined ? `page=${page}` : "",
        pageSize !== undefined ? `pageSize=${pageSize}` : ""
    ].filter(param => param !== "").join("&");
    const response = await Read<AdminArea[]>(
        `${process.env.API_URL}/api/geography?${queryParams}`,
        {
            method: "GET"
        },
        false
    );
    return response;
}

export async function reverseGeocode(longitude: number, latitude: number): Promise<ApiResponse<AdminArea[]>> {
    const response = await Mutate<AdminArea[]>(
        `${process.env.API_URL}/api/geography/reverse-geocode`,
        {
            method: "POST",
            body: JSON.stringify({ longitude, latitude }),
            headers: {
                "Content-Type": "application/json"
            }
        },
        false
    );
    return response;
}